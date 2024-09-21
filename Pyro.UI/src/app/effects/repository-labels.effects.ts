import { notifyAction } from '@actions/notification.actions';
import {
    createLabel,
    createLabelFailure,
    createLabelSuccess,
    disableLabel,
    disableLabelFailure,
    disableLabelSuccess,
    enableLabel,
    enableLabelFailure,
    enableLabelSuccess,
    loadLabels,
    loadLabelsFailure,
    loadLabelsSuccess,
    updateLabel,
    updateLabelFailure,
    updateLabelSuccess,
} from '@actions/repository-labels.actions';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { Store } from '@ngrx/store';
import { LabelService } from '@services/label.service';
import { AppState } from '@states/app.state';
import { selectLabels } from '@states/repository.state';
import { selectRouteParam } from '@states/router.state';
import { catchError, filter, map, of, switchMap, tap } from 'rxjs';

export const loadLabelsEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(LabelService),
    ) => {
        return actions$.pipe(
            ofType(loadLabels),
            concatLatestFrom(() => store.select(selectLabels)),
            filter(([_, labels]) => labels.loading),
            switchMap(([{ repositoryName }]) => service.getLabels(repositoryName)),
            map(labels => loadLabelsSuccess({ labels })),
            catchError(() => of(loadLabelsFailure())),
        );
    },
    { functional: true },
);

export const enableLabelEffect = createEffect(
    (actions$ = inject(Actions), service = inject(LabelService)) => {
        return actions$.pipe(
            ofType(enableLabel),
            switchMap(({ repositoryName, labelId }) =>
                service.enableLabel(repositoryName, labelId).pipe(map(() => labelId)),
            ),
            map(labelId => enableLabelSuccess({ labelId })),
            catchError(() => of(enableLabelFailure())),
        );
    },
    { functional: true },
);

export const enableLabelSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(enableLabelSuccess),
            map(() =>
                notifyAction({
                    severity: 'success',
                    title: 'Success',
                    message: 'Label enabled',
                }),
            ),
        );
    },
    { functional: true },
);

export const disableLabelEffect = createEffect(
    (actions$ = inject(Actions), service = inject(LabelService)) => {
        return actions$.pipe(
            ofType(disableLabel),
            switchMap(({ repositoryName, labelId }) =>
                service.disableLabel(repositoryName, labelId).pipe(map(() => labelId)),
            ),
            map(labelId => disableLabelSuccess({ labelId })),
            catchError(() => of(disableLabelFailure())),
        );
    },
    { functional: true },
);

export const disableLabelSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(disableLabelSuccess),
            map(() =>
                notifyAction({
                    severity: 'success',
                    title: 'Success',
                    message: 'Label disabled',
                }),
            ),
        );
    },
    { functional: true },
);

export const createLabelEffect = createEffect(
    (actions$ = inject(Actions), service = inject(LabelService)) => {
        return actions$.pipe(
            ofType(createLabel),
            switchMap(({ repositoryName, label }) => service.createLabel(repositoryName, label)),
            map(label => createLabelSuccess({ label })),
            catchError(() => of(createLabelFailure())),
        );
    },
    { functional: true },
);

export const updateLabelEffect = createEffect(
    (actions$ = inject(Actions), service = inject(LabelService)) => {
        return actions$.pipe(
            ofType(updateLabel),
            switchMap(({ repositoryName, labelId, label }) =>
                service.updateLabel(repositoryName, labelId, label),
            ),
            map(label => updateLabelSuccess({ label })),
            catchError(() => of(updateLabelFailure())),
        );
    },
    { functional: true },
);

export const createLabelSuccessEffect = createEffect(
    (actions$ = inject(Actions), router = inject(Router), store = inject(Store<AppState>)) => {
        return actions$.pipe(
            ofType(createLabelSuccess, updateLabelSuccess),
            concatLatestFrom(() => store.select(selectRouteParam('repositoryName'))),
            tap(([_, repositoryName]) =>
                router.navigate(['repositories', repositoryName, 'settings', 'labels']),
            ),
        );
    },
    { functional: true, dispatch: false },
);
