import {
    issuesNextPage,
    issuesPreviousPage,
    loadIssues,
    loadIssuesCurrentPageSuccess,
    loadIssuesNextPageSuccess,
    loadIssuesPreviousPageSuccess,
    loadIssueSuccess,
    loadIssueUsers,
    loadIssueUsersSuccess,
} from '@actions/issues.actions';
import {
    createLabelSuccess,
    disableLabelSuccess,
    enableLabelSuccess,
    loadLabels,
    loadLabelsSuccess,
    updateLabelSuccess,
} from '@actions/repository-labels.actions';
import {
    createStatusSuccess,
    createStatusTransitionSuccess,
    deleteStatusTransitionSuccess,
    disableStatusSuccess,
    enableStatusSuccess,
    loadStatuses,
    loadStatusesSuccess,
    loadStatusTransitions,
    loadStatusTransitionsSuccess,
    updateStatusSuccess,
} from '@actions/repository-statuses.actions';
import {
    loadBranchesSuccess,
    loadDirectoryViewSuccess,
    loadFileSuccess,
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
    statuses: { data: [], loading: false },
    statusTransitions: { data: [], loading: false },
    issues: {
        previous: { data: [], loading: false },
        current: { data: [], loading: false },
        next: { data: [], loading: false },
    },
    selectedIssue: null,
    users: { data: [], loading: false },
};

export const repositoryReducer = createReducer(
    initialState,
    on(
        loadRepositoryAndBranches,
        (state): RepositoryState => ({
            ...state,
            branches: {
                ...state.branches,
                loading: true,
            },
        }),
    ),
    on(
        loadRepositorySuccess,
        (state, { repository }): RepositoryState => ({
            ...state,
            repository,
        }),
    ),
    on(
        loadBranchesSuccess,
        (state, { branches }): RepositoryState => ({
            ...state,
            branches: {
                data: branches,
                loading: false,
            },
        }),
    ),
    on(setBranchOrPathSuccess, (state, { selectedBranch, branchOrPath }): RepositoryState => {
        return {
            ...state,
            branchOrPath,
            selectedBranch,
            directoryView: null,
            files: {},
        };
    }),
    on(
        loadDirectoryViewSuccess,
        (state, { directoryView }): RepositoryState => ({
            ...state,
            directoryView,
        }),
    ),
    on(
        loadFileSuccess,
        (state, { fileName, content }): RepositoryState => ({
            ...state,
            files: {
                ...state.files,
                [fileName]: content,
            },
        }),
    ),
    on(loadLabels, (state, { repositoryName }): RepositoryState => {
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
    on(
        loadLabelsSuccess,
        (state, { labels }): RepositoryState => ({
            ...state,
            labels: {
                data: labels,
                loading: false,
            },
        }),
    ),
    on(
        enableLabelSuccess,
        (state, { labelId }): RepositoryState => ({
            ...state,
            labels: {
                ...state.labels,
                data: state.labels.data.map(label =>
                    label.id === labelId ? { ...label, isDisabled: false } : label,
                ),
            },
        }),
    ),
    on(
        disableLabelSuccess,
        (state, { labelId }): RepositoryState => ({
            ...state,
            labels: {
                ...state.labels,
                data: state.labels.data.map(label =>
                    label.id === labelId ? { ...label, isDisabled: true } : label,
                ),
            },
        }),
    ),
    on(
        createLabelSuccess,
        (state, { label }): RepositoryState => ({
            ...state,
            labels: {
                ...state.labels,
                data: [...state.labels.data, label].sort((a, b) => a.name.localeCompare(b.name)),
            },
        }),
    ),
    on(
        updateLabelSuccess,
        (state, { label }): RepositoryState => ({
            ...state,
            labels: {
                ...state.labels,
                data: state.labels.data.map(l => (l.id === label.id ? label : l)),
            },
        }),
    ),
    on(loadStatuses, (state, { repositoryName }): RepositoryState => {
        if (
            (state.repository === null || state.repository.name === repositoryName) &&
            state.statuses.data.length > 0
        ) {
            return state;
        }

        return {
            ...state,
            statuses: {
                ...state.statuses,
                loading: true,
            },
        };
    }),
    on(
        loadStatusesSuccess,
        (state, { statuses }): RepositoryState => ({
            ...state,
            statuses: {
                data: statuses,
                loading: false,
            },
        }),
    ),
    on(
        enableStatusSuccess,
        (state, { statusId }): RepositoryState => ({
            ...state,
            statuses: {
                ...state.statuses,
                data: state.statuses.data.map(status =>
                    status.id === statusId ? { ...status, isDisabled: false } : status,
                ),
            },
        }),
    ),
    on(
        disableStatusSuccess,
        (state, { statusId }): RepositoryState => ({
            ...state,
            statuses: {
                ...state.statuses,
                data: state.statuses.data.map(status =>
                    status.id === statusId ? { ...status, isDisabled: true } : status,
                ),
            },
        }),
    ),
    on(
        createStatusSuccess,
        (state, { status }): RepositoryState => ({
            ...state,
            statuses: {
                ...state.statuses,
                data: [...state.statuses.data, status].sort((a, b) => a.name.localeCompare(b.name)),
            },
        }),
    ),
    on(
        updateStatusSuccess,
        (state, { status }): RepositoryState => ({
            ...state,
            statuses: {
                ...state.statuses,
                data: state.statuses.data.map(s => (s.id === status.id ? status : s)),
            },
            statusTransitions: { data: [], loading: false },
        }),
    ),
    on(loadStatusTransitions, (state, { repositoryName }): RepositoryState => {
        if (
            (state.repository === null || state.repository.name === repositoryName) &&
            state.statusTransitions.data.length > 0
        ) {
            return state;
        }

        return {
            ...state,
            statusTransitions: {
                ...state.statusTransitions,
                loading: true,
            },
        };
    }),
    on(
        deleteStatusTransitionSuccess,
        (state, { transitionId }): RepositoryState => ({
            ...state,
            statusTransitions: {
                ...state.statusTransitions,
                data: state.statusTransitions.data.filter(t => t.id !== transitionId),
            },
        }),
    ),
    on(
        loadStatusTransitionsSuccess,
        (state, { transitions }): RepositoryState => ({
            ...state,
            statusTransitions: {
                data: transitions,
                loading: false,
            },
        }),
    ),
    on(
        createStatusTransitionSuccess,
        (state, { transition }): RepositoryState => ({
            ...state,
            statusTransitions: {
                ...state.statusTransitions,
                data: [...state.statusTransitions.data, transition],
            },
        }),
    ),
    on(
        loadIssues,
        (state): RepositoryState => ({
            ...state,
            issues: {
                ...state.issues,
                current: { ...state.issues.current, loading: true },
            },
        }),
    ),
    on(
        loadIssuesCurrentPageSuccess,
        (state, { issues }): RepositoryState => ({
            ...state,
            issues: {
                ...state.issues,
                current: {
                    data: issues,
                    loading: false,
                },
            },
        }),
    ),
    on(
        loadIssuesPreviousPageSuccess,
        (state, { issues }): RepositoryState => ({
            ...state,
            issues: {
                ...state.issues,
                previous: {
                    data: issues,
                    loading: false,
                },
            },
        }),
    ),
    on(
        loadIssuesNextPageSuccess,
        (state, { issues }): RepositoryState => ({
            ...state,
            issues: {
                ...state.issues,
                next: {
                    data: issues,
                    loading: false,
                },
            },
        }),
    ),
    on(
        issuesPreviousPage,
        (state): RepositoryState => ({
            ...state,
            issues: {
                previous: { ...state.issues.previous, loading: true },
                current: state.issues.previous,
                next: state.issues.current,
            },
        }),
    ),
    on(
        issuesNextPage,
        (state): RepositoryState => ({
            ...state,
            issues: {
                previous: state.issues.current,
                current: state.issues.next,
                next: { ...state.issues.next, loading: true },
            },
        }),
    ),
    on(
        loadIssueUsers,
        (state): RepositoryState => ({
            ...state,
            users: {
                ...state.users,
                loading: true,
            },
        }),
    ),
    on(
        loadIssueUsersSuccess,
        (state, { users }): RepositoryState => ({
            ...state,
            users: {
                data: users,
                loading: false,
            },
        }),
    ),
    on(
        loadIssueSuccess,
        (state, { issue }): RepositoryState => ({
            ...state,
            selectedIssue: issue,
        }),
    ),
);
