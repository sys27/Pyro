import {
    editIssue,
    editIssueComponentOpened,
    editIssueFailure,
    editIssueSuccess,
    issuesNextPage,
    issuesPreviousPage,
    loadIssueFailure,
    loadIssues,
    loadIssuesCurrentPageSuccess,
    loadIssuesFailed,
    loadIssuesNextPageSuccess,
    loadIssuesPreviousPageSuccess,
    loadIssueSuccess,
    loadIssueUsers,
} from '@actions/issues.actions';
import { loadLabels } from '@actions/repository-labels.actions';
import { loadStatuses, loadStatusTransitions } from '@actions/repository-statuses.actions';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { Store } from '@ngrx/store';
import { IssueService } from '@services/issue.service';
import { AppState } from '@states/app.state';
import { selectCurrentPage } from '@states/paged.state';
import { selectIssues } from '@states/repository.state';
import { selectRouteParam } from '@states/router.state';
import { catchError, filter, map, of, switchMap, tap } from 'rxjs';

export const loadIssuesCurrentPageEffect = createEffect(
    (actions$ = inject(Actions), service = inject(IssueService)) => {
        return actions$.pipe(
            ofType(loadIssues),
            switchMap(({ repositoryName }) => service.getIssues(repositoryName)),
            map(issues => loadIssuesCurrentPageSuccess({ issues })),
            catchError(() => of(loadIssuesFailed())),
        );
    },
    { functional: true },
);

export const loadPreviousUsersPageEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(IssueService),
    ) => {
        return actions$.pipe(
            ofType(loadIssuesCurrentPageSuccess, issuesPreviousPage),
            concatLatestFrom(() => [
                store.select(selectRouteParam('repositoryName')),
                store.select(selectCurrentPage(selectIssues)),
            ]),
            filter(([_, repositoryName]) => !!repositoryName),
            switchMap(([_, repositoryName, issues]) => {
                let before: string | undefined;
                if (issues.data.length > 0) {
                    let firstItem = issues.data[0];
                    before = firstItem.id;

                    return service.getIssues(repositoryName!, before);
                }

                return of([]);
            }),
            map(issues => loadIssuesPreviousPageSuccess({ issues })),
            catchError(() => of(loadIssuesFailed())),
        );
    },
    { functional: true },
);

export const loadNextUsersPageEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(IssueService),
    ) => {
        return actions$.pipe(
            ofType(loadIssuesCurrentPageSuccess, issuesNextPage),
            concatLatestFrom(() => [
                store.select(selectRouteParam('repositoryName')),
                store.select(selectCurrentPage(selectIssues)),
            ]),
            filter(([_, repositoryName]) => !!repositoryName),
            switchMap(([_, repositoryName, issues]) => {
                let after: string | undefined;
                if (issues.data.length > 0) {
                    let lastItem = issues.data[issues.data.length - 1];
                    after = lastItem.id;

                    return service.getIssues(repositoryName!, undefined, after);
                }

                return of([]);
            }),
            map(issues => loadIssuesNextPageSuccess({ issues })),
            catchError(() => of(loadIssuesFailed())),
        );
    },
    { functional: true },
);

export const loadUserOnEditIssueComponentOpenedEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(editIssueComponentOpened),
            map(() => loadIssueUsers()),
        );
    },
    { functional: true },
);

export const loadLabelsOnEditIssueComponentOpenedEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(editIssueComponentOpened),
            map(({ repositoryName }) => loadLabels({ repositoryName })),
        );
    },
    { functional: true },
);

export const loadStatusesOnEditIssueComponentOpenedEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(editIssueComponentOpened),
            map(({ repositoryName }) => loadStatuses({ repositoryName })),
        );
    },
    { functional: true },
);

export const loadStatusTransitionsOnEditIssueComponentOpenedEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(editIssueComponentOpened),
            map(({ repositoryName }) => loadStatusTransitions({ repositoryName })),
        );
    },
    { functional: true },
);

export const loadIssueOnEditIssueComponentOpenedEffect = createEffect(
    (actions$ = inject(Actions), service = inject(IssueService)) => {
        return actions$.pipe(
            ofType(editIssueComponentOpened),
            switchMap(({ repositoryName, issueNumber }) =>
                service.getIssue(repositoryName, issueNumber),
            ),
            map(issue => loadIssueSuccess({ issue })),
            catchError(() => of(loadIssueFailure())),
        );
    },
    { functional: true },
);

export const editIssueEffect = createEffect(
    (actions$ = inject(Actions), service = inject(IssueService)) => {
        return actions$.pipe(
            ofType(editIssue),
            switchMap(({ repositoryName, issueNumber, issue }) =>
                service.updateIssue(repositoryName, issueNumber, issue),
            ),
            map(() => editIssueSuccess()),
            catchError(() => of(editIssueFailure())),
        );
    },
    { functional: true },
);

export const editIssueSuccessEffect = createEffect(
    (actions$ = inject(Actions), store = inject(Store<AppState>), router = inject(Router)) => {
        return actions$.pipe(
            ofType(editIssueSuccess),
            concatLatestFrom(() => store.select(selectRouteParam('repositoryName'))),
            tap(([_, repositoryName]) =>
                router.navigate(['repositories', repositoryName, 'issues']),
            ),
        );
    },
    { functional: true, dispatch: false },
);
