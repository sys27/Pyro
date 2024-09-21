import { refreshAction, refreshedAction } from '@actions/auth.actions';
import { HttpContextToken, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { selectCurrentUser } from '@states/auth.state';
import { catchError, map, of, switchMap, take, throwError } from 'rxjs';
import { Endpoints } from '../endpoints';

export const ALLOW_ANONYMOUS = new HttpContextToken<boolean>(() => false);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    let router = inject(Router);
    let store: Store<AppState> = inject(Store<AppState>);
    let actions$ = inject(Actions);

    return store.select(selectCurrentUser).pipe(
        take(1),
        switchMap(currentUser => {
            if (
                req.url !== Endpoints.Login &&
                req.url !== Endpoints.Logout &&
                req.url !== Endpoints.Refresh &&
                currentUser &&
                currentUser.expiresIn <= new Date()
            ) {
                store.dispatch(refreshAction({ refreshToken: currentUser.refreshToken }));

                // TODO: catch errors
                // TODO: handle multiple requests
                return actions$.pipe(
                    ofType(refreshedAction),
                    take(1),
                    map(({ currentUser }) => currentUser),
                    catchError(error => {
                        router.navigate(['/login']);

                        return throwError(() => error);
                    }),
                );
            }

            return of(currentUser);
        }),
        switchMap(currentUser => {
            if (currentUser) {
                let allowAnonymous = req.context.get(ALLOW_ANONYMOUS);
                if (!allowAnonymous) {
                    req = req.clone({
                        setHeaders: {
                            Authorization: `Bearer ${currentUser.accessToken}`,
                        },
                    });
                }
            }

            return next(req);
        }),
    );
};
