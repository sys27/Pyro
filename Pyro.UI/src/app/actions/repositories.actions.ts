import { createAction, props } from '@ngrx/store';
import { CreateRepository, RepositoryItem } from '@services/repository.service';

export const loadRepositories = createAction('[Repositories] Load Repositories');

export const loadRepositoriesPreviousPageSuccess = createAction(
    '[Repositories] Load Repositories Previous Page Success',
    props<{ repositories: RepositoryItem[] }>(),
);
export const loadRepositoriesCurrentPageSuccess = createAction(
    '[Repositories] Load Repositories Current Page Success',
    props<{ repositories: RepositoryItem[] }>(),
);
export const loadRepositoriesNextPageSuccess = createAction(
    '[Repositories] Load Repositories Next Page Success',
    props<{ repositories: RepositoryItem[] }>(),
);
export const loadRepositoriesFailure = createAction('[Repositories] Load Repositories Failure');

export const repositoriesPreviousPage = createAction('[Repositories] Previous Page');
export const repositoriesNextPage = createAction('[Repositories] Next Page');

export const createRepository = createAction(
    '[Repositories] Create Repository',
    props<{ repository: CreateRepository }>(),
);
export const createRepositorySuccess = createAction('[Repositories] Create Repository Success');
export const createRepositoryFailure = createAction('[Repositories] Create Repository Failure');
