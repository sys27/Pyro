import { Injectable, isDevMode } from '@angular/core';
import {
    HttpTransportType,
    HubConnection,
    HubConnectionBuilder,
    HubConnectionState,
    LogLevel,
} from '@microsoft/signalr';
import { MessageService } from 'primeng/api';
import { Observable, Subject, takeUntil } from 'rxjs';
import { AuthService } from './auth.service';

@Injectable({
    providedIn: 'root',
})
export class NotificationService {
    private readonly destroy$: Subject<void>;
    private readonly hubConnection: HubConnection;

    public constructor(messageService: MessageService, authService: AuthService) {
        this.destroy$ = new Subject<void>();
        this.hubConnection = new HubConnectionBuilder()
            .withUrl('/signalr', {
                accessTokenFactory: () => authService.getAccessToken()!,
                transport: HttpTransportType.WebSockets | HttpTransportType.ServerSentEvents,
            })
            .withAutomaticReconnect()
            .configureLogging(isDevMode() ? LogLevel.Trace : LogLevel.None)
            .build();

        authService.currentUser.pipe(takeUntil(this.destroy$)).subscribe(user => {
            if (user) {
                if (this.hubConnection.state === HubConnectionState.Disconnected) {
                    this.hubConnection.start().catch(error =>
                        messageService.add({
                            severity: 'error',
                            summary: 'Error',
                            detail: error,
                        }),
                    );
                }
            } else {
                this.hubConnection.stop();
            }
        });
    }

    public on<T>(eventName: string): Observable<T> {
        return new Observable<T>(subscriber => {
            let handler = (args: T) => subscriber.next(args);
            this.hubConnection.on(eventName, handler);

            return () => this.hubConnection.off(eventName, handler);
        });
    }

    public off(eventName: string): void {
        this.hubConnection.off(eventName);
    }

    public ngOnDestroy(): void {
        this.hubConnection.stop();
    }
}

export enum NotificationEvent {
    RepositoryInitialized = 'RepositoryInitialized',
}
