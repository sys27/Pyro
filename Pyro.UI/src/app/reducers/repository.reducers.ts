import {
    createLabelSuccess,
    disableLabelSuccess,
    enableLabelSuccess,
    loadBranchesSuccess,
    loadDirectoryViewSuccess,
    loadFileSuccess,
    loadLabels,
    loadLabelsSuccess,
    loadRepositoryAndBranches,
    loadRepositorySuccess,
    setBranchOrPathSuccess,
} from '@actions/repository.actions';
import { createReducer, on } from '@ngrx/store';
import { RepositoryState } from '@states/repository.state';

let initialState: RepositoryState = {
    repository: null,
    branches: { data: [], loading: false },
    branchOrPath: [],
    selectedBranch: null,
    directoryView: null,
    files: {},
    labels: { data: [], loading: false },
};

export const repositoryReducer = createReducer(
    initialState,
    on(loadRepositoryAndBranches, state => ({
        ...state,
        branches: {
            ...state.branches,
            loading: true,
        },
    })),
    on(loadRepositorySuccess, (state, { repository }) => ({
        ...state,
        repository,
    })),
    on(loadBranchesSuccess, (state, { branches }) => ({
        ...state,
        branches: {
            data: branches,
            loading: false,
        },
    })),
    on(setBranchOrPathSuccess, (state, { selectedBranch, branchOrPath }) => {
        return {
            ...state,
            branchOrPath,
            selectedBranch,
            directoryView: null,
            files: {},
        };
    }),
    on(loadDirectoryViewSuccess, (state, { directoryView }) => ({
        ...state,
        directoryView,
    })),
    on(loadFileSuccess, (state, { fileName, content }) => ({
        ...state,
        files: {
            ...state.files,
            [fileName]: content,
        },
    })),
    on(loadLabels, (state, { repositoryName }) => {
        if (
            (state.repository === null || state.repository.name === repositoryName) &&
            state.labels.data.length > 0
        ) {
            return state;
        }

        return {
            ...state,
            labels: {
                ...state.labels,
                loading: true,
            },
        };
    }),
    on(loadLabelsSuccess, (state, { labels }) => ({
        ...state,
        labels: {
            data: labels,
            loading: false,
        },
    })),
    on(enableLabelSuccess, (state, { labelId }) => ({
        ...state,
        labels: {
            ...state.labels,
            data: state.labels.data.map(label =>
                label.id === labelId ? { ...label, isDisabled: false } : label,
            ),
        },
    })),
    on(disableLabelSuccess, (state, { labelId }) => ({
        ...state,
        labels: {
            ...state.labels,
            data: state.labels.data.map(label =>
                label.id === labelId ? { ...label, isDisabled: true } : label,
            ),
        },
    })),
    on(createLabelSuccess, (state, { label }) => ({
        ...state,
        labels: {
            ...state.labels,
            data: [...state.labels.data, label].sort((a, b) => a.name.localeCompare(b.name)),
        },
    })),
);
