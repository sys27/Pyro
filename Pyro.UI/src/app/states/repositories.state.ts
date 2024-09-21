import { createFeatureSelector, createSelector } from '@ngrx/store';
import { RepositoryItem } from '@services/repository.service';
import { AppState } from './app.state';
import { PagedState } from './paged.state';

export interface RepositoryNewState {
    isProcessing: boolean;
}
export interface RepositoriesState extends PagedState<RepositoryItem> {
    newRepository: RepositoryNewState;
}

export const selectRepositoriesFeature = createFeatureSelector<RepositoriesState>('repositories');

export const selectIsNewRepositoryProcessing = createSelector<AppState, RepositoriesState, boolean>(
    selectRepositoriesFeature,
    state => state.newRepository.isProcessing,
);
