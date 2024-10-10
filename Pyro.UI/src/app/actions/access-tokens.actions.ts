import { createAction, props } from '@ngrx/store';
import { AccessToken, CreateAccessToken } from '@services/access-token.service';

export const loadAccessTokens = createAction('[AccessTokens] Load AccessTokens');
export const loadAccessTokensSuccess = createAction(
    '[AccessTokens] Load AccessTokens Success',
    props<{ accessTokens: AccessToken[] }>(),
);
export const loadAccessTokensFailure = createAction('[AccessTokens] Load AccessTokens Failure');

export const createAccessToken = createAction(
    '[AccessTokens] Create Access Token',
    props<{ accessToken: CreateAccessToken }>(),
);
export const createAccessTokenSuccess = createAction(
    '[AccessTokens] Create Access Token Success',
    props<{ accessToken: AccessToken }>(),
);
export const createAccessTokenFailure = createAction('[AccessTokens] Create Access Token Failure');

export const deleteAccessToken = createAction(
    '[AccessTokens] Delete Access Token',
    props<{ tokenName: string }>(),
);
export const deleteAccessTokenSuccess = createAction(
    '[AccessTokens] Delete Access Token Success',
    props<{ tokenName: string }>(),
);
export const deleteAccessTokenFailure = createAction('[AccessTokens] Delete Access Token Failure');
