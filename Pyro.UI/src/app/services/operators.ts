import { HttpErrorResponse, HttpStatusCode } from '@angular/common/http';
import { Injector } from '@angular/core';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { catchError, Observable, OperatorFunction, throwError } from 'rxjs';

export function createErrorHandler<T>(injector: Injector): OperatorFunction<T, T> {
    let router = injector.get(Router);
    let messageService = injector.get(MessageService);

    return (source: Observable<T>): Observable<T> => {
        return source.pipe(
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
                        detail: error.statusText,
                    });
                }

                return throwError(() => error);
            }),
        );
    };
}
