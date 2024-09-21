import { notifyAction } from '@actions/notification.actions';
import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { MessageService } from 'primeng/api';
import { tap } from 'rxjs';

export const notificationEffect = createEffect(
    (actions$ = inject(Actions), messageService = inject(MessageService)) => {
        return actions$.pipe(
            ofType(notifyAction),
            tap(notification =>
                messageService.add({
                    severity: notification.severity,
                    summary: notification.title,
                    detail: notification.message,
                }),
            ),
        );
    },
    { functional: true, dispatch: false },
);
