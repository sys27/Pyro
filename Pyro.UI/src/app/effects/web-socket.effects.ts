import { loggedInAction, loggedOutAction } from '@actions/auth.actions';
import { inject } from '@angular/core';
import { Actions, createEffect, ofType, ROOT_EFFECTS_INIT } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { Store } from '@ngrx/store';
import { WebSocketService } from '@services/web-socket.service';
import { AppState } from '@states/app.state';
import { selectCurrentUser } from '@states/auth.state';
import { filter, map, switchMap } from 'rxjs';

export const webSocketInitEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(WebSocketService),
    ) => {
        return actions$.pipe(
            ofType(ROOT_EFFECTS_INIT),
            concatLatestFrom(() => store.select(selectCurrentUser)),
            map(([_, currentUser]) => currentUser),
            filter(currentUser => !!currentUser),
            switchMap(currentUser => service.connect(currentUser)),
        );
    },
    { functional: true, dispatch: false },
);

export const webSocketConnectEffect = createEffect(
    (actions$ = inject(Actions), service = inject(WebSocketService)) => {
        return actions$.pipe(
            ofType(loggedInAction),
            switchMap(({ currentUser }) => service.connect(currentUser)),
        );
    },
    { functional: true, dispatch: false },
);

export const webSocketDisconnectEffect = createEffect(
    (actions$ = inject(Actions), service = inject(WebSocketService)) => {
        return actions$.pipe(
            ofType(loggedOutAction),
            switchMap(() => service.disconnect()),
        );
    },
    { functional: true, dispatch: false },
);
