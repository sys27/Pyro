import {
    createRepository,
    loadRepositories,
    loadRepositoriesCurrentPageSuccess,
    loadRepositoriesNextPageSuccess,
    loadRepositoriesPreviousPageSuccess,
    repositoriesNextPage,
    repositoriesPreviousPage,
} from '@actions/repositories.actions';
import { createReducer, on } from '@ngrx/store';
import { RepositoriesState } from '@states/repositories.state';

let initialState: RepositoriesState = {
    previous: { data: [], loading: false },
    current: { data: [], loading: false },
    next: { data: [], loading: false },
    newRepository: { isProcessing: false },
};

export const repositoriesReducer = createReducer(
    initialState,
    on(loadRepositories, state => ({
        ...state,
        current: { ...state.current, loading: true },
    })),
    on(loadRepositoriesPreviousPageSuccess, (state, { repositories }) => ({
        ...state,
        previous: { data: repositories, loading: false },
    })),
    on(loadRepositoriesCurrentPageSuccess, (state, { repositories }) => ({
        ...state,
        current: { data: repositories, loading: false },
    })),
    on(loadRepositoriesNextPageSuccess, (state, { repositories }) => ({
        ...state,
        next: { data: repositories, loading: false },
    })),
    on(repositoriesPreviousPage, state => ({
        ...state,
        previous: { ...state.previous, loading: true },
        current: state.previous,
        next: state.current,
    })),
    on(repositoriesNextPage, state => ({
        ...state,
        previous: state.current,
        current: state.next,
        next: { ...state.next, loading: true },
    })),
    on(createRepository, state => ({
        ...state,
        newRepository: { isProcessing: true },
    })),
);
