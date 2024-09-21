import { createFeatureSelector, createSelector } from '@ngrx/store';
import { Label } from '@services/label.service';
import { BranchItem, Repository, TreeView } from '@services/repository.service';
import { AppState } from './app.state';
import { DataSourceState } from './data-source.state';

export interface RepositoryState {
    repository: Repository | null;
    branches: DataSourceState<BranchItem>;
    branchOrPath: string[];
    selectedBranch: BranchItem | null;
    directoryView: TreeView | null;
    files: { [key: string]: string };
    labels: DataSourceState<Label>;
}

export const readmeFiles = ['readme.md', 'readme.txt', 'readme'];
export const licenseFiles = ['license.md', 'license.txt', 'license'];

export const repositoryStateSelector = createFeatureSelector<RepositoryState>('repository');
export const repositorySelector = createSelector<AppState, RepositoryState, Repository | null>(
    repositoryStateSelector,
    state => state.repository,
);
export const branchesSelector = createSelector<
    AppState,
    RepositoryState,
    DataSourceState<BranchItem>
>(repositoryStateSelector, state => state.branches);
export const branchOrPathSelector = createSelector<AppState, RepositoryState, string[]>(
    repositoryStateSelector,
    state => state.branchOrPath,
);
export const selectedBranchSelector = createSelector<AppState, RepositoryState, BranchItem | null>(
    repositoryStateSelector,
    state => state.selectedBranch,
);
export const directoryViewSelector = createSelector<AppState, RepositoryState, TreeView | null>(
    repositoryStateSelector,
    state => state.directoryView,
);
export const fileSelector = (fileName: string[]) =>
    createSelector<AppState, RepositoryState, string | null>(repositoryStateSelector, state => {
        if (!state.directoryView) {
            return null;
        }

        let file = state.directoryView.items.find(f => fileName.includes(f.name.toLowerCase()));
        if (!file) {
            return null;
        }

        return file.name;
    });
export const fileContentSelector = (fileName: string) =>
    createSelector<AppState, RepositoryState, string | null>(
        repositoryStateSelector,
        state => state.files[fileName] ?? null,
    );

export const labelsSelector = createSelector<AppState, RepositoryState, DataSourceState<Label>>(
    repositoryStateSelector,
    state => state.labels,
);
