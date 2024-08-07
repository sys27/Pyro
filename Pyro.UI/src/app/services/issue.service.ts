import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { Endpoints } from '../endpoints';
import { PyroResponse, ResponseError } from '../models/response';

@Injectable({ providedIn: 'root' })
export class IssueService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getIssues(repositoryName: string): Observable<PyroResponse<Issue[]>> {
        return this.httpClient
            .get<Issue[]>(Endpoints.Issues(repositoryName))
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public getIssue(repositoryName: string, issueNumber: number): Observable<PyroResponse<Issue>> {
        return this.httpClient
            .get<Issue>(`${Endpoints.Issues(repositoryName)}/${issueNumber}`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public getIssueComments(
        repositoryName: string,
        issueNumber: number,
    ): Observable<PyroResponse<Comment[]>> {
        return this.httpClient
            .get<Comment[]>(`${Endpoints.Issues(repositoryName)}/${issueNumber}/comments`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public createIssue(repositoryName: string, issue: CreateIssue): Observable<void> {
        let request = {
            title: issue.title,
            assigneeId: issue.assigneeId,
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
        };

        return this.httpClient.put<void>(
            `${Endpoints.Issues(repositoryName)}/${issueNumber}`,
            request,
        );
    }

    public createIssueComment(
        repositoryName: string,
        issueNumber: number,
        comment: CreateIssueComment,
    ): Observable<PyroResponse<Comment>> {
        let request = {
            content: comment.content,
        };

        return this.httpClient
            .post<Comment>(`${Endpoints.Issues(repositoryName)}/${issueNumber}/comments`, request)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public updateIssueComment(
        repositoryName: string,
        issueNumber: number,
        commentId: string,
        comment: UpdateIssueComment,
    ): Observable<PyroResponse<Comment>> {
        let request = {
            content: comment.content,
        };

        return this.httpClient.put<Comment>(
            `${Endpoints.Issues(repositoryName)}/${issueNumber}/comments/${commentId}`,
            request,
        );
    }

    public getUsers(): Observable<PyroResponse<User[]>> {
        return this.httpClient
            .get<User[]>(Endpoints.IssueUsers)
            .pipe(catchError((error: ResponseError) => of(error)));
    }
}

export interface Issue {
    issueNumber: number;
    title: string;
    author: User;
    createdAt: Date;
    assignee: User | null;
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
}

export interface UpdateIssue {
    title: string;
    assigneeId: string | null;
}

export interface CreateIssueComment {
    content: string;
}

export interface UpdateIssueComment {
    content: string;
}
