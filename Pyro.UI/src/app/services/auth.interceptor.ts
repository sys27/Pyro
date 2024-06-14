import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    let authService = inject(AuthService);
    let accessToken = authService.getAccessToken();
    if (accessToken) {
        req = req.clone({
            setHeaders: {
                Authorization: `Bearer ${accessToken}`,
            },
        });
    }

    return next(req);
};
