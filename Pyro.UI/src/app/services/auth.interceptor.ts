import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { of, switchMap, take } from 'rxjs';
import { Endpoints } from '../endpoints';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    let authService = inject(AuthService);

    return authService.currentUser.pipe(
        take(1),
        switchMap(currentUser => {
            if (
                req.url !== Endpoints.Refresh &&
                currentUser &&
                currentUser.expiresIn <= new Date()
            ) {
                return authService.refresh();
            }

            return of(currentUser);
        }),
        switchMap(currentUser => {
            if (currentUser) {
                req = req.clone({
                    setHeaders: {
                        Authorization: `Bearer ${currentUser.accessToken}`,
                    },
                });
            }

            return next(req);
        }),
    );
};
