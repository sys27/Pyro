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
import { Store } from '@ngrx/store';
import { RepositoryService } from '@services/repository.service';
import { AppState } from '@states/app.state';
import { currentPageSelector } from '@states/paged.state';
import { repositoriesSelector } from '@states/repositories.state';
import { catchError, map, of, switchMap, withLatestFrom } from 'rxjs';

export const loadRepositoriesEffect = createEffect(
    (actions$ = inject(Actions), service = inject(RepositoryService)) =>
        actions$.pipe(
            ofType(loadRepositories),
            switchMap(() => service.getRepositories()),
            map(repositories => loadRepositoriesCurrentPageSuccess({ repositories })),
            catchError(() => of(loadRepositoriesFailure())),
        ),
    { functional: true },
);

export const loadRepositoriesPreviousPageEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) =>
        actions$.pipe(
            ofType(loadRepositoriesCurrentPageSuccess),
            withLatestFrom(store.select(currentPageSelector(repositoriesSelector))),
            switchMap(([_, repositories]) => {
                let before: string | undefined;
                if (repositories.data.length > 0) {
                    let firstItem = repositories.data[0];
                    before = firstItem.name;
                }

                return service.getRepositories(before);
            }),
            map(repositories => loadRepositoriesPreviousPageSuccess({ repositories })),
            catchError(() => of(loadRepositoriesFailure())),
        ),
    { functional: true },
);

export const loadRepositoriesNextPageEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) =>
        actions$.pipe(
            ofType(loadRepositoriesCurrentPageSuccess),
            withLatestFrom(store.select(currentPageSelector(repositoriesSelector))),
            switchMap(([_, repositories]) => {
                let after: string | undefined;
                if (repositories.data.length > 0) {
                    let lastItem = repositories.data[repositories.data.length - 1];
                    after = lastItem.name;
                }

                return service.getRepositories(undefined, after);
            }),
            map(repositories => loadRepositoriesNextPageSuccess({ repositories })),
            catchError(() => of(loadRepositoriesFailure())),
        ),
    { functional: true },
);

export const createRepositoryEffect = createEffect(
    (actions$ = inject(Actions), service = inject(RepositoryService)) =>
        actions$.pipe(
            ofType(createRepository),
            switchMap(({ repository }) => service.createRepository(repository)),
            map(() => createRepositorySuccess()),
            catchError(() => of(createRepositoryFailure())),
        ),
    { functional: true },
);
