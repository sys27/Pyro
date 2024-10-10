import { createFeatureSelector, createSelector, select } from '@ngrx/store';
import { WebSocketEvents } from '@services/web-socket.service';
import { filter, pipe } from 'rxjs';
import { AppState } from './app.state';

export interface WebSocketState {
    message: WebSocketMessages | null;
}

export type WebSocketMessages = RepositoryInitializedMessage;

export interface WebSocketMessage {
    type: WebSocketEvents;
    payload: any;
}

export interface RepositoryInitializedMessage extends WebSocketMessage {
    type: WebSocketEvents.RepositoryInitialized;
    payload: {
        repositoryName: string;
    };
}

export const webSocketFeatureSelector = createFeatureSelector<WebSocketState>('webSocket');
export const messagesSelector = createSelector<AppState, WebSocketState, WebSocketMessages | null>(
    webSocketFeatureSelector,
    state => state.message,
);

type MessageTypeMap = {
    RepositoryInitialized: RepositoryInitializedMessage;
};
export const selectMessage = <T extends WebSocketEvents>(type: T) =>
    pipe(
        select(messagesSelector),
        filter((message): message is MessageTypeMap[T] => !!message && message.type === type),
    );
