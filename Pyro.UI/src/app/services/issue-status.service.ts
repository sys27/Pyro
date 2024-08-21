import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Color } from '@models/color';
import { PyroResponse, ResponseError } from '@models/response';
import { catchError, Observable, of } from 'rxjs';
import { Endpoints } from '../endpoints';

@Injectable({
    providedIn: 'root',
})
export class IssueStatusService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getStatuses(repositoryName: string): Observable<PyroResponse<IssueStatus[]>> {
        return this.httpClient
            .get<IssueStatus[]>(Endpoints.Statuses(repositoryName))
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public getStatus(repositoryName: string, id: string): Observable<PyroResponse<IssueStatus>> {
        return this.httpClient
            .get<IssueStatus>(`${Endpoints.Statuses(repositoryName)}/${id}`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public createStatus(
        repositoryName: string,
        status: CreateIssueStatus,
    ): Observable<PyroResponse<IssueStatus>> {
        let request = {
            name: status.name,
            color: status.color,
        };

        return this.httpClient
            .post<IssueStatus>(Endpoints.Statuses(repositoryName), request)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public updateStatus(
        repositoryName: string,
        id: string,
        status: UpdateIssueStatus,
    ): Observable<PyroResponse<IssueStatus>> {
        let request = {
            name: status.name,
            color: status.color,
        };

        return this.httpClient
            .put<IssueStatus>(`${Endpoints.Statuses(repositoryName)}/${id}`, request)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public deleteStatus(repositoryName: string, id: string): Observable<PyroResponse<void>> {
        return this.httpClient
            .delete<void>(`${Endpoints.Statuses(repositoryName)}/${id}`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public getStatusTransitions(
        repositoryName: string,
    ): Observable<PyroResponse<IssueStatusTransition[]>> {
        return this.httpClient
            .get<IssueStatusTransition[]>(Endpoints.StatusTransitions(repositoryName))
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public createStatusTransition(
        repositoryName: string,
        transition: CreateIssueStatusTransition,
    ): Observable<PyroResponse<IssueStatusTransition>> {
        let request = {
            fromId: transition.fromId,
            toId: transition.toId,
        };

        return this.httpClient
            .post<IssueStatusTransition>(Endpoints.StatusTransitions(repositoryName), request)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public deleteStatusTransition(
        repositoryName: string,
        id: string,
    ): Observable<PyroResponse<void>> {
        return this.httpClient
            .delete<void>(`${Endpoints.StatusTransitions(repositoryName)}/${id}`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }
}

export interface IssueStatus {
    id: string;
    name: string;
    color: Color;
}

export interface IssueStatusTransition {
    id: string;
    from: IssueStatus;
    to: IssueStatus;
}

export interface CreateIssueStatus {
    name: string;
    color: Color;
}

export interface UpdateIssueStatus {
    name: string;
    color: Color;
}

export interface CreateIssueStatusTransition {
    fromId: string;
    toId: string;
}
