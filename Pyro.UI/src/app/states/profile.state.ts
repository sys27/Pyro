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

export const profileSelector = createFeatureSelector<ProfileState>('profile');
export const currentProfileSelector = createSelector<AppState, ProfileState, Profile | null>(
    profileSelector,
    s => s.currentProfile,
);

export const accessTokensSelector = createSelector<
    AppState,
    ProfileState,
    DataSourceState<AccessToken>
>(profileSelector, s => s.accessTokens);

export const selectedAccessTokenSelector = createSelector<
    AppState,
    ProfileState,
    AccessToken | null
>(profileSelector, s => s.selectedAccessToken);
