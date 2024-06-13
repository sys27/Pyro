import { HttpHandlerFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from './auth.service';

export function authInterceptor(req: HttpRequest<unknown>, next: HttpHandlerFn) {
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
}
