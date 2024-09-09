import { HttpContextToken, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, of, switchMap, take, throwError } from 'rxjs';
import { Endpoints } from '../endpoints';
import { AuthService } from './auth.service';

export const ALLOW_ANONYMOUS = new HttpContextToken<boolean>(() => false);

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    let authService = inject(AuthService);
    let router = inject(Router);

    return authService.currentUser.pipe(
        take(1),
        switchMap(currentUser => {
            if (
                req.url !== Endpoints.Login &&
                req.url !== Endpoints.Logout &&
                req.url !== Endpoints.Refresh &&
                currentUser &&
                currentUser.expiresIn <= new Date()
            ) {
                // TODO: catch errors
                // TODO: handle multiple requests
                return authService.refresh().pipe(
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
