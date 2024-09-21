import { createAction, props } from '@ngrx/store';
import { CreateIssue, Issue, UpdateIssue, User } from '@services/issue.service';

export const loadIssues = createAction('[Issues] Load Issues', props<{ repositoryName: string }>());
export const loadIssuesCurrentPageSuccess = createAction(
    '[Issues] Load Issues Current Page Success',
    props<{ issues: Issue[] }>(),
);
export const loadIssuesPreviousPageSuccess = createAction(
    '[Issues] Load Issues Previous Page Success',
    props<{ issues: Issue[] }>(),
);
export const loadIssuesNextPageSuccess = createAction(
    '[Issues] Load Issues Next Page Success',
    props<{ issues: Issue[] }>(),
);
export const loadIssuesFailed = createAction('[Issues] Load Issues Failed');

export const issuesPreviousPage = createAction('[Issues] Previous Page');
export const issuesNextPage = createAction('[Issues] Next Page');

export const newIssueComponentOpened = createAction(
    '[Repository] New Issue Component Opened',
    props<{ repositoryName: string }>(),
);
export const createIssue = createAction(
    '[Repository] Create Issue',
    props<{ repositoryName: string; issue: CreateIssue }>(),
);
export const createIssueSuccess = createAction('[Repository] Create Issue Success');
export const createIssueFailure = createAction('[Repository] Create Issue Failure');

export const loadIssueUsers = createAction('[Repository] Load Users');
export const loadIssueUsersSuccess = createAction(
    '[Repository] Load Users Success',
    props<{ users: User[] }>(),
);
export const loadIssueUsersFailure = createAction('[Repository] Load Users Failure');

export const editIssueComponentOpened = createAction(
    '[Repository] Edit Issue Component Opened',
    props<{ repositoryName: string; issueNumber: number }>(),
);

export const editIssue = createAction(
    '[Repository] Edit Issue',
    props<{ repositoryName: string; issueNumber: number; issue: UpdateIssue }>(),
);
export const editIssueSuccess = createAction('[Repository] Edit Issue Success');
export const editIssueFailure = createAction('[Repository] Edit Issue Failure');

export const loadIssueSuccess = createAction(
    '[Repository] Load Issue Success',
    props<{ issue: Issue }>(),
);
export const loadIssueFailure = createAction('[Repository] Load Issue Failure');
