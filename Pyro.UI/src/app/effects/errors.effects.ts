import {
    createAccessTokenFailure,
    deleteAccessTokenFailure,
    loadAccessTokensFailure,
} from '@actions/access-tokens.actions';
import { loginFailedAction, refreshFailedAction } from '@actions/auth.actions';
import {
    createIssueFailure,
    editIssueFailure,
    loadIssueFailure,
    loadIssuesFailed,
    loadIssueUsersFailure,
} from '@actions/issues.actions';
import { loadProfileFailure, updateProfileFailure } from '@actions/profile.actions';
import { createRepositoryFailure, loadRepositoriesFailure } from '@actions/repositories.actions';
import {
    createLabelFailure,
    disableLabelFailure,
    enableLabelFailure,
    loadLabelsFailure,
    updateLabelFailure,
} from '@actions/repository-labels.actions';
import {
    createStatusFailure,
    createStatusTransitionFailure,
    deleteStatusTransitionFailure,
    disableStatusFailure,
    enableStatusFailure,
    loadStatusesFailure,
    loadStatusTransitionsFailure,
    updateStatusFailure,
} from '@actions/repository-statuses.actions';
import {
    loadBranchesFailure,
    loadDirectoryViewFailure,
    loadRepositoryFailure,
} from '@actions/repository.actions';
import { loadRolesFailure } from '@actions/roles.actions';
import {
    changePasswordFailure,
    createUserFailure,
    forgotPasswordFailure,
    loadUserFailure,
    loadUsersFailure,
    lockUserFailure,
    resetPasswordFailure,
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
                createLabelFailure,
                updateLabelFailure,
                loadStatusesFailure,
                enableStatusFailure,
                disableStatusFailure,
                createStatusFailure,
                updateStatusFailure,
                loadStatusTransitionsFailure,
                deleteStatusTransitionFailure,
                createStatusTransitionFailure,
                loadIssuesFailed,
                loadIssueUsersFailure,
                createIssueFailure,
                editIssueFailure,
                loadIssueFailure,
                changePasswordFailure,
                forgotPasswordFailure,
                resetPasswordFailure,
            ),
            tap(() =>
                messageService.add({
                    severity: 'error',
                    summary: 'Error',
                    detail: 'An error occurred',
                }),
            ),
        );
    },
    { dispatch: false, functional: true },
);
