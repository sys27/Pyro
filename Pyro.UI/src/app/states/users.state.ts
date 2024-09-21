import { createFeatureSelector, createSelector } from '@ngrx/store';
import { User, UserItem } from '@services/user.service';
import { AppState } from './app.state';
import { PagedState } from './paged.state';

export interface UserState extends PagedState<UserItem> {
    selectedUser: User | null;
}

export const selectUsers = createFeatureSelector<UserState>('users');
export const selectSelectedUser = createSelector<AppState, UserState, User | null>(
    selectUsers,
    state => state.selectedUser,
);
