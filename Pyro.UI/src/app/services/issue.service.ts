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
    id: string;
    issueNumber: number;
    title: string;
    author: User;
    createdAt: Date;
    assignee: User | null;
    labels: Label[];
    status: IssueStatus;
    isLocked: boolean;
}

export interface Comment {
    id: string;
    content: string;
    author: User;
    createdAt: Date;
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
    id: string;
    author: User;
    createdAt: Date;
    $type: IssueChangeLogType;
}

export interface IssueAssigneeChangeLog extends IssueChangeLog {
    oldAssignee: User | null;
    newAssignee: User | null;
    $type: IssueChangeLogType.Assignee;
}

export interface IssueLabelChangeLog extends IssueChangeLog {
    oldLabel: Label;
    newLabel: Label;
    $type: IssueChangeLogType.Label;
}

export interface IssueLockChangeLog extends IssueChangeLog {
    oldValue: boolean;
    newValue: boolean;
    $type: IssueChangeLogType.Lock;
}

export interface IssueStatusChangeLog extends IssueChangeLog {
    oldStatus: IssueStatus | null;
    newStatus: IssueStatus;
    $type: IssueChangeLogType.Status;
}

export interface IssueTitleChangeLog extends IssueChangeLog {
    oldTitle: string;
    newTitle: string;
    $type: IssueChangeLogType.Title;
}

export interface User {
    id: string;
    name: string;
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
