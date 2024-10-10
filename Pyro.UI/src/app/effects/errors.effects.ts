import {
    createAccessTokenFailure,
    deleteAccessTokenFailure,
    loadAccessTokensFailure,
} from '@actions/access-tokens.actions';
import { loginFailedAction, logoutFailedAction, refreshFailedAction } from '@actions/auth.actions';
import { loadProfileFailure, updateProfileFailure } from '@actions/profile.actions';
import { createRepositoryFailure, loadRepositoriesFailure } from '@actions/repositories.actions';
import {
    disableLabelFailure,
    enableLabelFailure,
    loadBranchesFailure,
    loadDirectoryViewFailure,
    loadLabelsFailure,
    loadRepositoryFailure,
} from '@actions/repository.actions';
import { loadRolesFailure } from '@actions/roles.actions';
import {
    createUserFailure,
    loadUserFailure,
    loadUsersFailure,
    lockUserFailure,
    unlockUserFailure,
    updateUserFailure,
} from '@actions/users.actions';
import { inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { MessageService } from 'primeng/api';
import { tap } from 'rxjs';

export const errorsEffect = createEffect(
    (actions$ = inject(Actions), messageService = inject(MessageService)) => {
        return actions$.pipe(
            ofType(
                loginFailedAction,
                logoutFailedAction,
                refreshFailedAction,
                loadRolesFailure,
                loadUsersFailure,
                lockUserFailure,
                unlockUserFailure,
                createUserFailure,
                updateUserFailure,
                loadUserFailure,
                loadProfileFailure,
                updateProfileFailure,
                loadAccessTokensFailure,
                createAccessTokenFailure,
                deleteAccessTokenFailure,
                loadRepositoriesFailure,
                createRepositoryFailure,
                loadRepositoryFailure,
                loadBranchesFailure,
                loadDirectoryViewFailure,
                loadLabelsFailure,
                enableLabelFailure,
                disableLabelFailure,
            ),
            tap(() =>
                messageService.add({
                    severity: 'error',
                    summary: 'Error',
                    detail: 'An error occurred', // TODO: message
                }),
            ),
        );
    },
    { dispatch: false, functional: true },
);
