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
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { AuthService } from '@services/auth.service';
import { catchError, map, of, switchMap, tap } from 'rxjs';

export const loginEffect = createEffect(
    (actions$ = inject(Actions), authService = inject(AuthService)) => {
        return actions$.pipe(
            ofType(loginAction),
            switchMap(({ login, password, returnUrl }) =>
                authService
                    .login(login, password)
                    .pipe(map(currentUser => ({ currentUser, returnUrl }))),
            ),
            map(({ currentUser, returnUrl }) => loggedInAction({ currentUser, returnUrl })),
            catchError(() => of(loginFailedAction())),
        );
    },
    { functional: true },
);

export const loggedInActionEffect = createEffect(
    (actions$ = inject(Actions), router = inject(Router)) => {
        return actions$.pipe(
            ofType(loggedInAction),
            tap(({ returnUrl }) => router.navigateByUrl(sanitizeReturnUrl(returnUrl))),
        );
    },
    { functional: true, dispatch: false },
);

function sanitizeReturnUrl(returnUrl: string): string {
    returnUrl ??= '/';

    const currentUrl = window.location.origin;

    let url = new URL(returnUrl, currentUrl);
    if (url.origin !== currentUrl) {
        return '/';
    }

    return url.pathname;
}

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

export const loggedOutActionEffect = createEffect(
    (actions$ = inject(Actions), router = inject(Router)) => {
        return actions$.pipe(
            ofType(loggedOutAction, logoutFailedAction),
            tap(() => router.navigate(['/login'])),
        );
    },
    { functional: true, dispatch: false },
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
