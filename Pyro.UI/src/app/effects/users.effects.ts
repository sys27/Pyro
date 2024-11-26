import { logoutAction } from '@actions/auth.actions';
import { notifyAction } from '@actions/notification.actions';
import { loadRoles } from '@actions/roles.actions';
import {
    changePassword,
    changePasswordFailure,
    changePasswordSuccess,
    createUser,
    createUserFailure,
    createUserSuccess,
    forgotPassword,
    forgotPasswordFailure,
    forgotPasswordSuccess,
    loadCurrentUsersPageSuccess,
    loadNextUsersPageSuccess,
    loadPreviousUsersPageSuccess,
    loadUser,
    loadUserFailure,
    loadUsers,
    loadUsersFailure,
    loadUserSuccess,
    lockUser,
    lockUserFailure,
    lockUserSuccess,
    resetPassword,
    resetPasswordFailure,
    resetPasswordSuccess,
    unlockUser,
    unlockUserFailure,
    unlockUserSuccess,
    updateUser,
    updateUserFailure,
    updateUserSuccess,
    usersNextPage,
    usersPreviousPage,
} from '@actions/users.actions';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { Store } from '@ngrx/store';
import { ChangePassword, UserService } from '@services/user.service';
import { AppState } from '@states/app.state';
import { selectCurrentPage } from '@states/paged.state';
import { selectUsers } from '@states/users.state';
import { catchError, map, of, switchMap, tap } from 'rxjs';

export const loadCurrentUsersPageEffect = createEffect(
    (actions$ = inject(Actions), userService = inject(UserService)) => {
        return actions$.pipe(
            ofType(loadUsers),
            switchMap(() => userService.getUsers()),
            map(users => loadCurrentUsersPageSuccess({ users })),
            catchError(() => of(loadUsersFailure())),
        );
    },
    { functional: true },
);

export const loadPreviousUsersPageEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        userService = inject(UserService),
    ) => {
        return actions$.pipe(
            ofType(loadCurrentUsersPageSuccess, usersPreviousPage),
            concatLatestFrom(() => store.select(selectCurrentPage(selectUsers))),
            switchMap(([_, users]) => {
                let before: string | undefined;
                if (users.data.length > 0) {
                    let firstItem = users.data[0];
                    before = firstItem.login;

                    return userService.getUsers(before);
                }

                return of([]);
            }),
            map(users => loadPreviousUsersPageSuccess({ users })),
            catchError(() => of(loadUsersFailure())),
        );
    },
    { functional: true },
);

export const loadNextUsersPageEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        userService = inject(UserService),
    ) => {
        return actions$.pipe(
            ofType(loadCurrentUsersPageSuccess, usersNextPage),
            concatLatestFrom(() => store.select(selectCurrentPage(selectUsers))),
            switchMap(([_, users]) => {
                let after: string | undefined;
                if (users.data.length > 0) {
                    let lastItem = users.data[users.data.length - 1];
                    after = lastItem.login;

                    return userService.getUsers(undefined, after);
                }

                return of([]);
            }),
            map(users => loadNextUsersPageSuccess({ users })),
            catchError(() => of(loadUsersFailure())),
        );
    },
    { functional: true },
);

export const lockUserEffect = createEffect(
    (actions$ = inject(Actions), userService = inject(UserService)) => {
        return actions$.pipe(
            ofType(lockUser),
            switchMap(({ login }) => userService.lockUser(login).pipe(map(() => ({ login })))),
            map(({ login }) => lockUserSuccess({ login })),
            catchError(() => of(lockUserFailure())),
        );
    },
    {
        functional: true,
    },
);
export const unlockUserEffect = createEffect(
    (actions$ = inject(Actions), userService = inject(UserService)) => {
        return actions$.pipe(
            ofType(unlockUser),
            switchMap(({ login }) => userService.unlockUser(login).pipe(map(() => ({ login })))),
            map(({ login }) => unlockUserSuccess({ login })),
            catchError(() => of(unlockUserFailure())),
        );
    },
    { functional: true },
);

export const createUserEffect = createEffect(
    (actions$ = inject(Actions), userService = inject(UserService)) => {
        return actions$.pipe(
            ofType(createUser),
            switchMap(({ user }) => userService.createUser(user).pipe(map(() => user))),
            map(user => createUserSuccess({ login: user.login })),
            catchError(() => of(createUserFailure())),
        );
    },
    { functional: true },
);

export const createUserSuccessEffect = createEffect(
    (actions$ = inject(Actions), router = inject(Router)) => {
        return actions$.pipe(
            ofType(createUserSuccess),
            map(action =>
                notifyAction({
                    title: 'User created',
                    message: `User '${action.login}' has been created`,
                    severity: 'success',
                }),
            ),
            tap(() => router.navigate(['/users'])),
        );
    },
    { functional: true },
);

export const updateUserEffect = createEffect(
    (actions$ = inject(Actions), userService = inject(UserService)) => {
        return actions$.pipe(
            ofType(updateUser),
            switchMap(({ login, user }) =>
                userService.updateUser(login, user).pipe(map(() => login)),
            ),
            map(login => updateUserSuccess({ login })),
            catchError(() => of(updateUserFailure())),
        );
    },
    { functional: true },
);

export const updateUserSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(updateUserSuccess),
            map(action =>
                notifyAction({
                    title: 'User updated',
                    message: `User '${action.login}' has been updated`,
                    severity: 'success',
                }),
            ),
        );
    },
    { functional: true },
);

export const loadRolesForUserEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(loadUser),
            map(() => loadRoles()),
        );
    },
    { functional: true },
);

export const loadUserEffect = createEffect(
    (actions$ = inject(Actions), service = inject(UserService)) => {
        return actions$.pipe(
            ofType(loadUser),
            switchMap(({ login }) => service.getUser(login)),
            map(user => loadUserSuccess({ user })),
            catchError(() => of(loadUserFailure())),
        );
    },
    { functional: true },
);

export const changePasswordEffect = createEffect(
    (actions$ = inject(Actions), userService = inject(UserService)) => {
        return actions$.pipe(
            ofType(changePassword),
            switchMap(({ oldPassword, newPassword }) => {
                let command: ChangePassword = { oldPassword, newPassword };

                return userService.changePassword(command);
            }),
            map(() => changePasswordSuccess()),
            catchError(() => of(changePasswordFailure())),
        );
    },
    { functional: true },
);

export const changePasswordSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(changePasswordSuccess),
            map(() =>
                notifyAction({
                    title: 'Password changed',
                    message: 'Password has been changed',
                    severity: 'success',
                }),
            ),
        );
    },
    { functional: true },
);

export const forgotPasswordEffect = createEffect(
    (actions$ = inject(Actions), userService = inject(UserService)) => {
        return actions$.pipe(
            ofType(forgotPassword),
            switchMap(({ login }) => userService.forgotPassword(login)),
            map(() => forgotPasswordSuccess()),
            catchError(() => of(forgotPasswordFailure())),
        );
    },
    { functional: true },
);

export const forgotPasswordSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(forgotPasswordSuccess),
            map(() =>
                notifyAction({
                    severity: 'success',
                    title: 'Success',
                    message: 'Password reset link has been sent to email',
                }),
            ),
        );
    },
    { functional: true, dispatch: false },
);

export const resetPasswordEffect = createEffect(
    (actions$ = inject(Actions), userService = inject(UserService)) => {
        return actions$.pipe(
            ofType(resetPassword),
            switchMap(({ token, password }) => userService.resetPassword(token, password)),
            map(() => resetPasswordSuccess()),
            catchError(() => of(resetPasswordFailure())),
        );
    },
    { functional: true },
);

export const resetPasswordSuccessEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(resetPasswordSuccess),
            map(() => logoutAction()),
        );
    },
    { functional: true },
);
