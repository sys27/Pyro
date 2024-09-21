import {
    createAccessToken,
    createAccessTokenFailure,
    createAccessTokenSuccess,
    deleteAccessToken,
    deleteAccessTokenFailure,
    deleteAccessTokenSuccess,
    loadAccessTokens,
    loadAccessTokensFailure,
    loadAccessTokensSuccess,
} from '@actions/access-tokens.actions';
import { notifyAction } from '@actions/notification.actions';
import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { AccessTokenService } from '@services/access-token.service';
import { catchError, map, of, switchMap } from 'rxjs';

export const loadAccessTokensEffect = createEffect(
    (actions$ = inject(Actions), service = inject(AccessTokenService)) => {
        return actions$.pipe(
            ofType(loadAccessTokens),
            switchMap(() => service.getAccessTokens()),
            map(accessTokens => loadAccessTokensSuccess({ accessTokens })),
            catchError(() => of(loadAccessTokensFailure())),
        );
    },
    { functional: true },
);

export const createAccessTokenEffect = createEffect(
    (actions$ = inject(Actions), service = inject(AccessTokenService)) => {
        return actions$.pipe(
            ofType(createAccessToken),
            switchMap(({ accessToken }) => service.createAccessToken(accessToken)),
            map(accessToken => createAccessTokenSuccess({ accessToken })),
            catchError(() => of(createAccessTokenFailure())),
        );
    },
    { functional: true },
);

export const deleteAccessTokenEffect = createEffect(
    (actions$ = inject(Actions), service = inject(AccessTokenService)) => {
        return actions$.pipe(
            ofType(deleteAccessToken),
            switchMap(({ tokenName }) =>
                service.deleteAccessToken(tokenName).pipe(map(() => tokenName)),
            ),
            map(tokenName => deleteAccessTokenSuccess({ tokenName })),
            catchError(() => of(deleteAccessTokenFailure())),
        );
    },
    { functional: true },
);

export const deleteAccessTokenSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(deleteAccessTokenSuccess),
            map(action =>
                notifyAction({
                    title: 'Access token deleted',
                    message: `Access token ${action.tokenName} deleted`,
                    severity: 'success',
                }),
            ),
        );
    },
    { functional: true },
);
