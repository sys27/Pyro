import { HttpErrorResponse, HttpInterceptorFn, HttpStatusCode } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { ResponseError } from '../models/response';

export const errorHandlingInterceptor: HttpInterceptorFn = (req, next) => {
    let router = inject(Router);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === HttpStatusCode.Unauthorized) {
                router.navigate(['/login']);
            }

            if (
                error.status >= HttpStatusCode.BadRequest &&
                error.status <= HttpStatusCode.NetworkAuthenticationRequired
            ) {
                // TODO:
                console.error(error);
            }

            return throwError(() => error as ResponseError);
        }),
    );
};
