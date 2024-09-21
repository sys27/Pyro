import { createFeatureSelector, createSelector } from '@ngrx/store';
import { IssueStatus, IssueStatusTransition } from '@services/issue-status.service';
import { Issue, User } from '@services/issue.service';
import { Label } from '@services/label.service';
import { BranchItem, Repository, TreeView } from '@services/repository.service';
import { AppState } from './app.state';
import { DataSourceState } from './data-source.state';
import { PagedState } from './paged.state';

export interface RepositoryState {
    repository: Repository | null;
    branches: DataSourceState<BranchItem>;
    branchOrPath: string[];
    selectedBranch: BranchItem | null;
    directoryView: TreeView | null;
    files: Record<string, string>;
    labels: DataSourceState<Label>;
    statuses: DataSourceState<IssueStatus>;
    statusTransitions: DataSourceState<IssueStatusTransition>;
    issues: PagedState<Issue>;
    selectedIssue: Issue | null;
    users: DataSourceState<User>;
}

export const readmeFiles = ['readme.md', 'readme.txt', 'readme'];
export const licenseFiles = ['license.md', 'license.txt', 'license'];

export const selectRepositoryFeature = createFeatureSelector<RepositoryState>('repository');
export const selectRepository = createSelector<AppState, RepositoryState, Repository | null>(
    selectRepositoryFeature,
    state => state.repository,
);
export const selectBranches = createSelector<
    AppState,
    RepositoryState,
    DataSourceState<BranchItem>
>(selectRepositoryFeature, state => state.branches);
export const selectBranchOrPath = createSelector<AppState, RepositoryState, string[]>(
    selectRepositoryFeature,
    state => state.branchOrPath,
);
export const selectSelectedBranch = createSelector<AppState, RepositoryState, BranchItem | null>(
    selectRepositoryFeature,
    state => state.selectedBranch,
);
export const selectDirectoryView = createSelector<AppState, RepositoryState, TreeView | null>(
    selectRepositoryFeature,
    state => state.directoryView,
);
export const selectFile = (fileName: string[]) =>
    createSelector<AppState, RepositoryState, string | null>(selectRepositoryFeature, state => {
        if (!state.directoryView) {
            return null;
        }

        let file = state.directoryView.items.find(f => fileName.includes(f.name.toLowerCase()));
        if (!file) {
            return null;
        }

        return file.name;
    });
export const selectFileContent = (fileName: string) =>
    createSelector<AppState, RepositoryState, string | null>(
        selectRepositoryFeature,
        state => state.files[fileName] ?? null,
    );

export const selectLabels = createSelector<AppState, RepositoryState, DataSourceState<Label>>(
    selectRepositoryFeature,
    state => state.labels,
);
export const selectEnabledLabels = createSelector<
    AppState,
    RepositoryState,
    DataSourceState<Label>
>(selectRepositoryFeature, state => ({
    data: state.labels.data.filter(label => !label.isDisabled),
    loading: state.labels.loading,
}));

export const selectStatuses = createSelector<
    AppState,
    RepositoryState,
    DataSourceState<IssueStatus>
>(selectRepositoryFeature, state => state.statuses);
export const selectEnabledStatuses = createSelector<
    AppState,
    RepositoryState,
    DataSourceState<IssueStatus>
>(selectRepositoryFeature, state => ({
    data: state.statuses.data.filter(status => !status.isDisabled),
    loading: state.statuses.loading,
}));

export const selectStatusTransitions = createSelector<
    AppState,
    RepositoryState,
    DataSourceState<IssueStatusTransition>
>(selectRepositoryFeature, state => state.statusTransitions);

export const selectIssues = createSelector<AppState, RepositoryState, PagedState<Issue>>(
    selectRepositoryFeature,
    state => state.issues,
);

export const selectUsers = createSelector<AppState, RepositoryState, DataSourceState<User>>(
    selectRepositoryFeature,
    state => state.users,
);

export const selectSelectedIssue = createSelector<AppState, RepositoryState, Issue | null>(
    selectRepositoryFeature,
    state => state.selectedIssue,
);
