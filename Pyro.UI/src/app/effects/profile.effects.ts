import { notifyAction } from '@actions/notification.actions';
import {
    loadProfile,
    loadProfileFailure,
    loadProfileSuccess,
    updateProfile,
    updateProfileFailure,
    updateProfileSuccess,
} from '@actions/profile.actions';
import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { ProfileService } from '@services/profile.service';
import { catchError, map, of, switchMap } from 'rxjs';

export const loadProfileEffect = createEffect(
    (actions$ = inject(Actions), profileService = inject(ProfileService)) => {
        return actions$.pipe(
            ofType(loadProfile),
            switchMap(() => profileService.getProfile()),
            map(profile => loadProfileSuccess({ profile })),
            catchError(() => of(loadProfileFailure())),
        );
    },
    { functional: true },
);

export const updateProfileEffect = createEffect(
    (actions$ = inject(Actions), profileService = inject(ProfileService)) => {
        return actions$.pipe(
            ofType(updateProfile),
            switchMap(({ profile }) => profileService.updateProfile(profile)),
            map(() => updateProfileSuccess()),
            catchError(() => of(updateProfileFailure())),
        );
    },
    { functional: true },
);

export const updateProfileSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(updateProfileSuccess),
            map(() =>
                notifyAction({
                    title: 'Profile updated',
                    message: 'Profile has been updated',
                    severity: 'success',
                }),
            ),
        );
    },
    { functional: true },
);
