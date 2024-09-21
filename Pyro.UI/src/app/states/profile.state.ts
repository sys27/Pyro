import { createFeatureSelector, createSelector } from '@ngrx/store';
import { AccessToken } from '@services/access-token.service';
import { Profile } from '@services/profile.service';
import { AppState } from './app.state';
import { DataSourceState } from './data-source.state';

export interface ProfileState {
    currentProfile: Profile | null;
    accessTokens: DataSourceState<AccessToken>;
    selectedAccessToken: AccessToken | null;
}

export const selectProfile = createFeatureSelector<ProfileState>('profile');
export const selectCurrentProfile = createSelector<AppState, ProfileState, Profile | null>(
    selectProfile,
    s => s.currentProfile,
);

export const selectAccessTokens = createSelector<
    AppState,
    ProfileState,
    DataSourceState<AccessToken>
>(selectProfile, s => s.accessTokens);

export const selectSelectedAccessToken = createSelector<AppState, ProfileState, AccessToken | null>(
    selectProfile,
    s => s.selectedAccessToken,
);
