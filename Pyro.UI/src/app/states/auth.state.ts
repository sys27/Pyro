import { CurrentUser } from '@models/current-user';
import { PyroPermissions } from '@models/pyro-permissions';
import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AppState } from './app.state';

export interface AuthState {
    currentUser: CurrentUser | null;
}

export const selectAuthFeature = createFeatureSelector<AuthState>('auth');
export const selectCurrentUser = createSelector<AppState, AuthState, CurrentUser | null>(
    selectAuthFeature,
    s => s.currentUser,
);
export const selectIsLoggedIn = createSelector<AppState, AuthState, boolean>(
    selectAuthFeature,
    s => s.currentUser !== null,
);
export const selectHasPermission = (permission: PyroPermissions) =>
    createSelector<AppState, AuthState, boolean>(
        selectAuthFeature,
        s => s.currentUser?.permissions.includes(permission) ?? false,
    );
