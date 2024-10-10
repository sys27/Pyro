import { CurrentUser } from '@models/current-user';
import { PyroPermissions } from '@models/pyro-permissions';
import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AppState } from './app.state';

export interface AuthState {
    currentUser: CurrentUser | null;
}

export const authSelector = createFeatureSelector<AuthState>('auth');
export const currentUserSelector = createSelector<AppState, AuthState, CurrentUser | null>(
    authSelector,
    s => s.currentUser,
);
export const isLoggedInSelector = createSelector<AppState, AuthState, boolean>(
    authSelector,
    s => s.currentUser !== null,
);
export const hasPermissionSelector = (permission: PyroPermissions) =>
    createSelector<AppState, AuthState, boolean>(
        authSelector,
        s => s.currentUser?.permissions.includes(permission) ?? false,
    );
