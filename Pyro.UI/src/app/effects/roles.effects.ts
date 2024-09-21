import { loadRoles, loadRolesFailure, loadRolesSuccess } from '@actions/roles.actions';
import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { Store } from '@ngrx/store';
import { UserService } from '@services/user.service';
import { AppState } from '@states/app.state';
import { selectRolesFeature } from '@states/roles.state';
import { catchError, filter, map, of, switchMap } from 'rxjs';

export const loadRolesEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        userService = inject(UserService),
    ) => {
        return actions$.pipe(
            ofType(loadRoles),
            concatLatestFrom(() => store.select(selectRolesFeature)),
            filter(([_, state]) => state.data.length === 0),
            switchMap(() => userService.getRoles()),
            map(roles => loadRolesSuccess({ roles })),
            catchError(() => of(loadRolesFailure())),
        );
    },
    { functional: true },
);
