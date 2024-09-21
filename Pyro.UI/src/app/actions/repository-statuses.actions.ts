import { createAction, props } from '@ngrx/store';
import {
    CreateIssueStatus,
    CreateIssueStatusTransition,
    IssueStatus,
    IssueStatusTransition,
    UpdateIssueStatus,
} from '@services/issue-status.service';

export const loadStatuses = createAction(
    '[Repository Statuses] Load Statuses',
    props<{ repositoryName: string }>(),
);
export const loadStatusesSuccess = createAction(
    '[Repository Statuses] Load Statuses Success',
    props<{ statuses: IssueStatus[] }>(),
);
export const loadStatusesFailure = createAction('[Repository Statuses] Load Statuses Failure');

export const enableStatus = createAction(
    '[Repository Statuses] Enable Status',
    props<{ repositoryName: string; statusId: string }>(),
);
export const enableStatusSuccess = createAction(
    '[Repository Statuses] Enable Status Success',
    props<{ statusId: string }>(),
);
export const enableStatusFailure = createAction('[Repository Statuses] Enable Status Failure');

export const disableStatus = createAction(
    '[Repository Statuses] Disable Status',
    props<{ repositoryName: string; statusId: string }>(),
);
export const disableStatusSuccess = createAction(
    '[Repository Statuses] Disable Status Success',
    props<{ statusId: string }>(),
);
export const disableStatusFailure = createAction('[Repository Statuses] Disable Status Failure');

export const createStatus = createAction(
    '[Repository Statuses] Create Status',
    props<{ repositoryName: string; status: CreateIssueStatus }>(),
);
export const createStatusSuccess = createAction(
    '[Repository Statuses] Create Status Success',
    props<{ status: IssueStatus }>(),
);
export const createStatusFailure = createAction('[Repository Statuses] Create Status Failure');

export const updateStatus = createAction(
    '[Repository Statuses] Update Status',
    props<{ repositoryName: string; statusId: string; status: UpdateIssueStatus }>(),
);
export const updateStatusSuccess = createAction(
    '[Repository Statuses] Update Status Success',
    props<{ status: IssueStatus }>(),
);
export const updateStatusFailure = createAction('[Repository Statuses] Update Status Failure');

export const loadStatusTransitions = createAction(
    '[Repository Status Transitions] Load Status Transitions',
    props<{ repositoryName: string }>(),
);
export const loadStatusTransitionsSuccess = createAction(
    '[Repository Status Transitions] Load Status Transitions Success',
    props<{ transitions: IssueStatusTransition[] }>(),
);
export const loadStatusTransitionsFailure = createAction(
    '[Repository Status Transitions] Load Status Transitions Failure',
);

export const deleteStatusTransition = createAction(
    '[Repository Status Transitions] Delete Transition',
    props<{ repositoryName: string; transitionId: string }>(),
);
export const deleteStatusTransitionSuccess = createAction(
    '[Repository Status Transitions] Delete Transition Success',
    props<{ transitionId: string }>(),
);
export const deleteStatusTransitionFailure = createAction(
    '[Repository Status Transitions] Delete Transition Failure',
);

export const createStatusTransition = createAction(
    '[Repository Status Transitions] Create Transition',
    props<{ repositoryName: string; transition: CreateIssueStatusTransition }>(),
);
export const createStatusTransitionSuccess = createAction(
    '[Repository Status Transitions] Create Transition Success',
    props<{ transition: IssueStatusTransition }>(),
);
export const createStatusTransitionFailure = createAction(
    '[Repository Status Transitions] Create Transition Failure',
);
