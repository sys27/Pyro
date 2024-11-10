import { refreshAction, refreshedAction, refreshFailedAction } from '@actions/auth.actions';
import { HttpContextToken, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Actions, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { selectCurrentUser } from '@states/auth.state';
import { map, of, race, switchMap, take } from 'rxjs';
import { Endpoints } from '../endpoints';

export const ALLOW_ANONYMOUS = new HttpContextToken<boolean>(() => false);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
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
                return race(
                    actions$.pipe(
                        ofType(refreshedAction),
                        map(({ currentUser }) => currentUser),
                    ),
                    actions$.pipe(
                        ofType(refreshFailedAction),
                        map(() => null),
                    ),
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
