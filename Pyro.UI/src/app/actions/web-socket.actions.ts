import { createAction, props } from '@ngrx/store';
import { WebSocketMessages } from '@states/web-socket.state';

export const webSocketMessageReceived = createAction(
    '[WebSocket] Message Received',
    props<{ message: WebSocketMessages }>(),
);
