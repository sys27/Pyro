import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';
import { IssueStatus } from './issue-status.service';
import { Label } from './label.service';

@Injectable({ providedIn: 'root' })
export class IssueService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getIssues(repositoryName: string, before?: string, after?: string): Observable<Issue[]> {
        let params = new HttpParams().set('size', 20);

        if (before) {
            params = params.set('before', before);
        }

        if (after) {
            params = params.set('after', after);
        }

        return this.httpClient.get<Issue[]>(Endpoints.Issues(repositoryName), { params: params });
    }

    public getIssue(repositoryName: string, issueNumber: number): Observable<Issue> {
        return this.httpClient.get<Issue>(`${Endpoints.Issues(repositoryName)}/${issueNumber}`);
    }

    public getIssueComments(repositoryName: string, issueNumber: number): Observable<Comment[]> {
        return this.httpClient.get<Comment[]>(
            `${Endpoints.Issues(repositoryName)}/${issueNumber}/comments`,
        );
    }

    public createIssue(repositoryName: string, issue: CreateIssue): Observable<void> {
        let request = {
            title: issue.title,
            assigneeId: issue.assigneeId,
            labels: issue.labels,
            statusId: issue.statusId,
            initialComment: issue.initialComment,
        };

        return this.httpClient.post<void>(Endpoints.Issues(repositoryName), request);
    }

    public updateIssue(
        repositoryName: string,
        issueNumber: number,
        issue: UpdateIssue,
    ): Observable<void> {
        let request = {
            title: issue.title,
            assigneeId: issue.assigneeId,
            labels: issue.labels,
            statusId: issue.statusId,
        };

        return this.httpClient.put<void>(
            `${Endpoints.Issues(repositoryName)}/${issueNumber}`,
            request,
        );
    }

    public lockIssue(repositoryName: string, issueNumber: number): Observable<void> {
        return this.httpClient.post<void>(
            `${Endpoints.Issues(repositoryName)}/${issueNumber}/lock`,
            null,
        );
    }

    public unlockIssue(repositoryName: string, issueNumber: number): Observable<void> {
        return this.httpClient.post<void>(
            `${Endpoints.Issues(repositoryName)}/${issueNumber}/unlock`,
            null,
        );
    }

    public createIssueComment(
        repositoryName: string,
        issueNumber: number,
        comment: CreateIssueComment,
    ): Observable<Comment> {
        let request = {
            content: comment.content,
        };

        return this.httpClient.post<Comment>(
            `${Endpoints.Issues(repositoryName)}/${issueNumber}/comments`,
            request,
        );
    }

    public updateIssueComment(
        repositoryName: string,
        issueNumber: number,
        commentId: string,
        comment: UpdateIssueComment,
    ): Observable<Comment> {
        let request = {
            content: comment.content,
        };

        return this.httpClient.put<Comment>(
            `${Endpoints.Issues(repositoryName)}/${issueNumber}/comments/${commentId}`,
            request,
        );
    }

    public getUsers(): Observable<User[]> {
        return this.httpClient.get<User[]>(Endpoints.IssueUsers);
    }

    public getIssueChangeLogs(
        repositoryName: string,
        issueNumber: number,
    ): Observable<ChangeLogs[]> {
        return this.httpClient.get<ChangeLogs[]>(
            Endpoints.IssueChangeLogs(repositoryName, issueNumber),
        );
    }
}

export interface Issue {
    get id(): string;
    get issueNumber(): number;
    get title(): string;
    get author(): User;
    get createdAt(): Date;
    get assignee(): User | null;
    get labels(): Label[];
    get status(): IssueStatus;
    get isLocked(): boolean;
}

export interface Comment {
    get id(): string;
    get content(): string;
    get author(): User;
    get createdAt(): Date;
}

export type ChangeLogs =
    | IssueAssigneeChangeLog
    | IssueLabelChangeLog
    | IssueLockChangeLog
    | IssueStatusChangeLog
    | IssueTitleChangeLog;

export enum IssueChangeLogType {
    Assignee = 1,
    Label = 2,
    Lock = 3,
    Status = 4,
    Title = 5,
}

export interface IssueChangeLog {
    get id(): string;
    get author(): User;
    get createdAt(): Date;
    get $type(): IssueChangeLogType;
}

export interface IssueAssigneeChangeLog extends IssueChangeLog {
    get oldAssignee(): User | null;
    get newAssignee(): User | null;
    get $type(): IssueChangeLogType.Assignee;
}

export interface IssueLabelChangeLog extends IssueChangeLog {
    get oldLabel(): Label;
    get newLabel(): Label;
    get $type(): IssueChangeLogType.Label;
}

export interface IssueLockChangeLog extends IssueChangeLog {
    get oldValue(): boolean;
    get newValue(): boolean;
    get $type(): IssueChangeLogType.Lock;
}

export interface IssueStatusChangeLog extends IssueChangeLog {
    get oldStatus(): IssueStatus | null;
    get newStatus(): IssueStatus;
    get $type(): IssueChangeLogType.Status;
}

export interface IssueTitleChangeLog extends IssueChangeLog {
    get oldTitle(): string;
    get newTitle(): string;
    get $type(): IssueChangeLogType.Title;
}

export interface User {
    get id(): string;
    get displayName(): string;
}

export interface CreateIssue {
    title: string;
    assigneeId: string | null;
    labels: string[];
    statusId: string;
    initialComment: string;
}

export interface UpdateIssue {
    title: string;
    assigneeId: string | null;
    labels: string[];
    statusId: string;
}

export interface CreateIssueComment {
    content: string;
}

export interface UpdateIssueComment {
    content: string;
}
