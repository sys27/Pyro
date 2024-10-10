import { createAction, props } from '@ngrx/store';
import { WebSocketMessage } from '@states/web-socket.state';

export const webSocketMessageReceived = createAction(
    '[WebSocket] Message Received',
    props<{ message: WebSocketMessage }>(),
);
