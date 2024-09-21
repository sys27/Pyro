import {
    loggedInAction,
    loggedOutAction,
    loginAction,
    loginFailedAction,
    logoutAction,
    logoutFailedAction,
    refreshAction,
    refreshedAction,
    refreshFailedAction,
} from '@actions/auth.actions';
import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { AuthService } from '@services/auth.service';
import { catchError, map, of, switchMap } from 'rxjs';

export const loginEffect = createEffect(
    (actions$ = inject(Actions), authService = inject(AuthService)) => {
        return actions$.pipe(
            ofType(loginAction),
            switchMap(({ login, password }) => authService.login(login, password)),
            map(currentUser => loggedInAction({ currentUser })),
            catchError(() => of(loginFailedAction())),
        );
    },
    { functional: true },
);

export const logoutEffect = createEffect(
    (actions$ = inject(Actions), authService = inject(AuthService)) => {
        return actions$.pipe(
            ofType(logoutAction, loginFailedAction, refreshFailedAction),
            switchMap(() => authService.logout()),
            map(() => loggedOutAction()),
            catchError(() => of(logoutFailedAction())),
        );
    },
    { functional: true },
);

export const refreshEffect = createEffect(
    (actions$ = inject(Actions), authService = inject(AuthService)) => {
        return actions$.pipe(
            ofType(refreshAction),
            switchMap(({ refreshToken }) => authService.refresh(refreshToken)),
            map(currentUser => refreshedAction({ currentUser })),
            catchError(() => of(refreshFailedAction())),
        );
    },
    { functional: true },
);
