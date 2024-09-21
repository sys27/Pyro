import {
    createAccessTokenSuccess,
    deleteAccessTokenSuccess,
    loadAccessTokens,
    loadAccessTokensSuccess,
} from '@actions/access-tokens.actions';
import { logoutAction } from '@actions/auth.actions';
import { loadProfileSuccess } from '@actions/profile.actions';
import { routerNavigatedAction } from '@ngrx/router-store';
import { createReducer, on } from '@ngrx/store';
import { ProfileState } from '@states/profile.state';

let initialState: ProfileState = {
    currentProfile: null,
    accessTokens: { data: [], loading: false },
    selectedAccessToken: null,
};
export const profileReducer = createReducer(
    initialState,
    on(loadProfileSuccess, (state, { profile }) => ({ ...state, currentProfile: profile })),
    on(loadAccessTokens, state => ({
        ...state,
        accessTokens: { ...state.accessTokens, loading: true },
        selectedAccessToken: null,
    })),
    on(loadAccessTokensSuccess, (state, { accessTokens }) => ({
        ...state,
        accessTokens: {
            data: accessTokens,
            loading: false,
        },
        selectedAccessToken: null,
    })),
    on(createAccessTokenSuccess, (state, { accessToken }) => ({
        ...state,
        selectedAccessToken: accessToken,
    })),
    on(deleteAccessTokenSuccess, (state, { tokenName }) => ({
        ...state,
        accessTokens: {
            data: state.accessTokens.data.filter(t => t.name !== tokenName),
            loading: false,
        },
        selectedAccessToken: null,
    })),

    on(routerNavigatedAction, state => ({
        ...state,
        selectedAccessToken: null,
    })),
    on(logoutAction, () => initialState),
);
