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
    payload: unknown;
}

export interface RepositoryInitializedMessage extends WebSocketMessage {
    type: WebSocketEvents.RepositoryInitialized;
    payload: {
        repositoryName: string;
    };
}

export const selectWebSocketFeature = createFeatureSelector<WebSocketState>('webSocket');
export const selectMessages = createSelector<AppState, WebSocketState, WebSocketMessages | null>(
    selectWebSocketFeature,
    state => state.message,
);

interface MessageTypeMap {
    RepositoryInitialized: RepositoryInitializedMessage;
}
export const selectMessage = <T extends WebSocketEvents>(type: T) =>
    pipe(
        select(selectMessages),
        filter((message): message is MessageTypeMap[T] => !!message && message.type === type),
    );
