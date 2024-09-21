import { loggedInAction, loggedOutAction } from '@actions/auth.actions';
import { inject } from '@angular/core';
import { Actions, createEffect, ofType, ROOT_EFFECTS_INIT } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { WebSocketService } from '@services/web-socket.service';
import { AppState } from '@states/app.state';
import { currentUserSelector } from '@states/auth.state';
import { filter, map, switchMap, withLatestFrom } from 'rxjs';

export const webSocketInitEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(WebSocketService),
    ) =>
        actions$.pipe(
            ofType(ROOT_EFFECTS_INIT),
            withLatestFrom(store.select(currentUserSelector)),
            map(([_, currentUser]) => currentUser),
            filter(currentUser => !!currentUser),
            switchMap(currentUser => service.connect(currentUser)),
        ),
    { functional: true, dispatch: false },
);

export const webSocketConnectEffect = createEffect(
    (actions$ = inject(Actions), service = inject(WebSocketService)) =>
        actions$.pipe(
            ofType(loggedInAction),
            switchMap(({ currentUser }) => service.connect(currentUser)),
        ),
    { functional: true, dispatch: false },
);

export const webSocketDisconnectEffect = createEffect(
    (actions$ = inject(Actions), service = inject(WebSocketService)) =>
        actions$.pipe(
            ofType(loggedOutAction),
            switchMap(() => service.disconnect()),
        ),
    { functional: true, dispatch: false },
);
