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

export interface User {
    id: string;
    name: string;
}

export interface CreateIssue {
    title: string;
    assigneeId: string | null;
    labels: string[];
    statusId: string;
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
