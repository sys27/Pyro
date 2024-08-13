import { HttpErrorResponse, HttpInterceptorFn, HttpStatusCode } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { ResponseError } from '@models/response';
import { MessageService } from 'primeng/api';
import { catchError, throwError } from 'rxjs';

export const errorHandlingInterceptor: HttpInterceptorFn = (req, next) => {
    let router = inject(Router);
    let messageService = inject(MessageService);

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === HttpStatusCode.Unauthorized) {
                router.navigate(['/login']);
            }

            if (
                error.status >= HttpStatusCode.BadRequest &&
                error.status <= HttpStatusCode.NetworkAuthenticationRequired
            ) {
                messageService.add({
                    severity: 'error',
                    summary: 'Error',
                    detail: error.error?.title,
                });
            }

            return throwError(() => new ResponseError(error.message));
        }),
    );
};
