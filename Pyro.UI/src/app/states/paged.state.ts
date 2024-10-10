import { createSelector, Selector } from '@ngrx/store';
import { AppState } from './app.state';
import { DataSourceState } from './data-source.state';

export interface PagedState<T> {
    previous: DataSourceState<T>;
    current: DataSourceState<T>;
    next: DataSourceState<T>;
}

export const previousPageSelector = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, DataSourceState<T>>(featureSelector, s => s.previous);
export const currentPageSelector = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, DataSourceState<T>>(featureSelector, s => s.current);
export const nextPageSelector = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, DataSourceState<T>>(featureSelector, s => s.next);

export const hasPreviousSelector = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, boolean>(
        featureSelector,
        s => s.previous.data.length > 0,
    );
export const hasNextSelector = <T>(featureSelector: Selector<AppState, PagedState<T>>) =>
    createSelector<AppState, PagedState<T>, boolean>(featureSelector, s => s.next.data.length > 0);
