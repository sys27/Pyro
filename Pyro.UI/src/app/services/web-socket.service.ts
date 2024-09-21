import { webSocketMessageReceived } from '@actions/web-socket.actions';
import { Injectable, isDevMode } from '@angular/core';
import {
    HttpTransportType,
    HubConnection,
    HubConnectionBuilder,
    LogLevel,
} from '@microsoft/signalr';
import { CurrentUser } from '@models/current-user';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';

@Injectable({
    providedIn: 'root',
})
export class WebSocketService {
    private hubConnection: HubConnection | undefined;

    public constructor(private readonly store: Store<AppState>) {}

    public async connect(currentUser: CurrentUser): Promise<void> {
        this.hubConnection = new HubConnectionBuilder()
            .withUrl('/signalr', {
                accessTokenFactory: () => currentUser.accessToken,
                transport: HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents,
            })
            .withAutomaticReconnect()
            .configureLogging(isDevMode() ? LogLevel.Trace : LogLevel.None)
            .build();

        await this.hubConnection.start();
        this.hubConnection.on(WebSocketEvents.RepositoryInitialized, (repositoryName: string) => {
            this.store.dispatch(
                webSocketMessageReceived({
                    message: {
                        type: WebSocketEvents.RepositoryInitialized,
                        payload: { repositoryName },
                    },
                }),
            );
        });
    }

    public disconnect(): Promise<void> {
        if (this.hubConnection) {
            return this.hubConnection.stop();
        }

        return Promise.resolve();
    }
}

export enum WebSocketEvents {
    RepositoryInitialized = 'RepositoryInitialized',
}
