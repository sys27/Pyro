import { createAction, props } from '@ngrx/store';

export type NotificationSeverity = 'info' | 'success' | 'warn' | 'error' | 'secondary' | 'contrast';
export const notifyAction = createAction(
    '[Notification] Notification Action',
    props<{ title: string; message: string; severity: NotificationSeverity }>(),
);
