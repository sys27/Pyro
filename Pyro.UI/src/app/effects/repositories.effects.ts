import {
    createRepository,
    createRepositoryFailure,
    createRepositorySuccess,
    loadRepositories,
    loadRepositoriesCurrentPageSuccess,
    loadRepositoriesFailure,
    loadRepositoriesNextPageSuccess,
    loadRepositoriesPreviousPageSuccess,
} from '@actions/repositories.actions';
import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { Store } from '@ngrx/store';
import { RepositoryService } from '@services/repository.service';
import { AppState } from '@states/app.state';
import { selectCurrentPage } from '@states/paged.state';
import { selectRepositoriesFeature } from '@states/repositories.state';
import { catchError, map, of, switchMap } from 'rxjs';

export const loadRepositoriesEffect = createEffect(
    (actions$ = inject(Actions), service = inject(RepositoryService)) => {
        return actions$.pipe(
            ofType(loadRepositories),
            switchMap(() => service.getRepositories()),
            map(repositories => loadRepositoriesCurrentPageSuccess({ repositories })),
            catchError(() => of(loadRepositoriesFailure())),
        );
    },
    { functional: true },
);

export const loadRepositoriesPreviousPageEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) => {
        return actions$.pipe(
            ofType(loadRepositoriesCurrentPageSuccess),
            concatLatestFrom(() => store.select(selectCurrentPage(selectRepositoriesFeature))),
            switchMap(([_, repositories]) => {
                let before: string | undefined;
                if (repositories.data.length > 0) {
                    let firstItem = repositories.data[0];
                    before = firstItem.name;

                    return service.getRepositories(before);
                }

                return of([]);
            }),
            map(repositories => loadRepositoriesPreviousPageSuccess({ repositories })),
            catchError(() => of(loadRepositoriesFailure())),
        );
    },
    { functional: true },
);

export const loadRepositoriesNextPageEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) => {
        return actions$.pipe(
            ofType(loadRepositoriesCurrentPageSuccess),
            concatLatestFrom(() => store.select(selectCurrentPage(selectRepositoriesFeature))),
            switchMap(([_, repositories]) => {
                let after: string | undefined;
                if (repositories.data.length > 0) {
                    let lastItem = repositories.data[repositories.data.length - 1];
                    after = lastItem.name;

                    return service.getRepositories(undefined, after);
                }

                return of([]);
            }),
            map(repositories => loadRepositoriesNextPageSuccess({ repositories })),
            catchError(() => of(loadRepositoriesFailure())),
        );
    },
    { functional: true },
);

export const createRepositoryEffect = createEffect(
    (actions$ = inject(Actions), service = inject(RepositoryService)) => {
        return actions$.pipe(
            ofType(createRepository),
            switchMap(({ repository }) => service.createRepository(repository)),
            map(() => createRepositorySuccess()),
            catchError(() => of(createRepositoryFailure())),
        );
    },
    { functional: true },
);
