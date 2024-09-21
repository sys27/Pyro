import { webSocketMessageReceived } from '@actions/web-socket.actions';
import { createReducer, on } from '@ngrx/store';
import { WebSocketState } from '@states/web-socket.state';

let initialState: WebSocketState = {
    message: null,
};
export const webSocketReducer = createReducer(
    initialState,
    on(
        webSocketMessageReceived,
        (state, { message }): WebSocketState => ({
            ...state,
            message: message,
        }),
    ),
);
