import { notifyAction } from '@actions/notification.actions';
import {
    createStatus,
    createStatusFailure,
    createStatusSuccess,
    createStatusTransition,
    createStatusTransitionFailure,
    createStatusTransitionSuccess,
    deleteStatusTransition,
    deleteStatusTransitionFailure,
    deleteStatusTransitionSuccess,
    disableStatus,
    disableStatusFailure,
    disableStatusSuccess,
    enableStatus,
    enableStatusFailure,
    enableStatusSuccess,
    loadStatuses,
    loadStatusesFailure,
    loadStatusesSuccess,
    loadStatusTransitions,
    loadStatusTransitionsFailure,
    loadStatusTransitionsSuccess,
    updateStatus,
    updateStatusFailure,
    updateStatusSuccess,
} from '@actions/repository-statuses.actions';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { Store } from '@ngrx/store';
import { IssueStatusService } from '@services/issue-status.service';
import { AppState } from '@states/app.state';
import { selectStatuses, selectStatusTransitions } from '@states/repository.state';
import { selectRouteParam } from '@states/router.state';
import { catchError, filter, map, of, switchMap, tap } from 'rxjs';

export const loadStatusesEffect = createEffect(
    (
        actions$ = inject(Actions),
        services = inject(IssueStatusService),
        store = inject(Store<AppState>),
    ) => {
        return actions$.pipe(
            ofType(loadStatuses),
            concatLatestFrom(() => store.select(selectStatuses)),
            filter(([_, statuses]) => statuses.loading),
            switchMap(([{ repositoryName }]) => services.getStatuses(repositoryName)),
            map(statuses => loadStatusesSuccess({ statuses })),
            catchError(() => of(loadStatusesFailure())),
        );
    },
    { functional: true },
);

export const enableStatusEffect = createEffect(
    (actions$ = inject(Actions), services = inject(IssueStatusService)) => {
        return actions$.pipe(
            ofType(enableStatus),
            switchMap(({ repositoryName, statusId }) =>
                services.enableStatus(repositoryName, statusId).pipe(map(() => statusId)),
            ),
            map(statusId => enableStatusSuccess({ statusId })),
            catchError(() => of(enableStatusFailure())),
        );
    },
    { functional: true },
);

export const enableStatusSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(enableStatusSuccess),
            map(() =>
                notifyAction({
                    severity: 'success',
                    title: 'Success',
                    message: 'Status enabled',
                }),
            ),
        );
    },
    { functional: true },
);

export const disableStatusEffect = createEffect(
    (actions$ = inject(Actions), services = inject(IssueStatusService)) => {
        return actions$.pipe(
            ofType(disableStatus),
            switchMap(({ repositoryName, statusId }) =>
                services.disableStatus(repositoryName, statusId).pipe(map(() => statusId)),
            ),
            map(statusId => disableStatusSuccess({ statusId })),
            catchError(() => of(disableStatusFailure())),
        );
    },
    { functional: true },
);

export const disableStatusSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(disableStatusSuccess),
            map(() =>
                notifyAction({
                    severity: 'success',
                    title: 'Success',
                    message: 'Status disabled',
                }),
            ),
        );
    },
    { functional: true },
);

export const createStatusEffect = createEffect(
    (actions$ = inject(Actions), services = inject(IssueStatusService)) => {
        return actions$.pipe(
            ofType(createStatus),
            switchMap(({ repositoryName, status }) =>
                services.createStatus(repositoryName, status),
            ),
            map(status => createStatusSuccess({ status })),
            catchError(() => of(createStatusFailure())),
        );
    },
    { functional: true },
);

export const updateStatusEffect = createEffect(
    (actions$ = inject(Actions), services = inject(IssueStatusService)) => {
        return actions$.pipe(
            ofType(updateStatus),
            switchMap(({ repositoryName, statusId, status }) =>
                services.updateStatus(repositoryName, statusId, status),
            ),
            map(status => updateStatusSuccess({ status })),
            catchError(() => of(updateStatusFailure())),
        );
    },
    { functional: true },
);

export const createUpdateStatusSuccessEffect = createEffect(
    (actions$ = inject(Actions), router = inject(Router), store = inject(Store<AppState>)) => {
        return actions$.pipe(
            ofType(createStatusSuccess, updateStatusSuccess),
            concatLatestFrom(() => store.select(selectRouteParam('repositoryName'))),
            tap(([_, repositoryName]) =>
                router.navigate(['repositories', repositoryName, 'settings', 'statuses']),
            ),
        );
    },
    { functional: true, dispatch: false },
);

export const loadStatusTransitionsEffect = createEffect(
    (
        actions$ = inject(Actions),
        service = inject(IssueStatusService),
        store = inject(Store<AppState>),
    ) => {
        return actions$.pipe(
            ofType(loadStatusTransitions),
            concatLatestFrom(() => store.select(selectStatusTransitions)),
            filter(([_, transitions]) => transitions.loading),
            switchMap(([{ repositoryName }]) => service.getStatusTransitions(repositoryName)),
            map(transitions => loadStatusTransitionsSuccess({ transitions })),
            catchError(() => of(loadStatusTransitionsFailure())),
        );
    },
    { functional: true },
);

export const deleteStatusTransitionEffect = createEffect(
    (actions$ = inject(Actions), service = inject(IssueStatusService)) => {
        return actions$.pipe(
            ofType(deleteStatusTransition),
            switchMap(({ repositoryName, transitionId }) =>
                service
                    .deleteStatusTransition(repositoryName, transitionId)
                    .pipe(map(() => transitionId)),
            ),
            map(transitionId => deleteStatusTransitionSuccess({ transitionId })),
            catchError(() => of(deleteStatusTransitionFailure())),
        );
    },
    { functional: true },
);

export const deleteStatusTransitionSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(deleteStatusTransitionSuccess),
            map(() =>
                notifyAction({
                    severity: 'success',
                    title: 'Success',
                    message: 'Status transition deleted',
                }),
            ),
        );
    },
    { functional: true },
);

export const createStatusTransitionEffect = createEffect(
    (actions$ = inject(Actions), service = inject(IssueStatusService)) => {
        return actions$.pipe(
            ofType(createStatusTransition),
            switchMap(({ repositoryName, transition }) =>
                service.createStatusTransition(repositoryName, transition),
            ),
            map(transition => createStatusTransitionSuccess({ transition })),
            catchError(() => of(createStatusTransitionFailure())),
        );
    },
    { functional: true },
);

export const createStatusTransitionSuccessEffect = createEffect(
    (actions$ = inject(Actions), router = inject(Router), store = inject(Store<AppState>)) => {
        return actions$.pipe(
            ofType(createStatusTransitionSuccess),
            concatLatestFrom(() => store.select(selectRouteParam('repositoryName'))),
            tap(([_, repositoryName]) =>
                router.navigate([
                    'repositories',
                    repositoryName,
                    'settings',
                    'statuses',
                    'transitions',
                ]),
            ),
        );
    },
    { functional: true, dispatch: false },
);
