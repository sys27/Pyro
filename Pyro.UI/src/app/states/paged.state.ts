import { createSelector, Selector } from '@ngrx/store';
import { AppState } from './app.state';
import { DataSourceState } from './data-source.state';

export interface PagedState<T> {
    previous: DataSourceState<T>;
    current: DataSourceState<T>;
    next: DataSourceState<T>;
}

export const selectPreviousPage = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, DataSourceState<T>>(featureSelector, s => s.previous);
export const selectCurrentPage = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, DataSourceState<T>>(featureSelector, s => s.current);
export const selectNextPage = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, DataSourceState<T>>(featureSelector, s => s.next);

export const selectHasPrevious = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, boolean>(
        featureSelector,
        s => s.previous.data.length > 0,
    );
export const selectHasNext = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, boolean>(featureSelector, s => s.next.data.length > 0);
