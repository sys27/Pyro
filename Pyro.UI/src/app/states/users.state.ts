import { createFeatureSelector, createSelector } from '@ngrx/store';
import { User, UserItem } from '@services/user.service';
import { AppState } from './app.state';
import { PagedState } from './paged.state';

export interface UserState extends PagedState<UserItem> {
    selectedUser: User | null;
}

export const usersSelector = createFeatureSelector<UserState>('users');
export const selectedUserSelector = createSelector<AppState, UserState, User | null>(
    usersSelector,
    state => state.selectedUser,
);
