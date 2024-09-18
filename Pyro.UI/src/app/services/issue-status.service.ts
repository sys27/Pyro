import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Color } from '@models/color';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';

@Injectable({
    providedIn: 'root',
})
export class IssueStatusService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getStatuses(repositoryName: string, statusName?: string): Observable<IssueStatus[]> {
        let httpParams = new HttpParams();
        if (statusName) {
            httpParams = httpParams.set('statusName', statusName);
        }

        return this.httpClient.get<IssueStatus[]>(Endpoints.Statuses(repositoryName), {
            params: httpParams,
        });
    }

    public getStatus(repositoryName: string, id: string): Observable<IssueStatus> {
        return this.httpClient.get<IssueStatus>(`${Endpoints.Statuses(repositoryName)}/${id}`);
    }

    public createStatus(
        repositoryName: string,
        status: CreateIssueStatus,
    ): Observable<IssueStatus> {
        let request = {
            name: status.name,
            color: status.color,
        };

        return this.httpClient.post<IssueStatus>(Endpoints.Statuses(repositoryName), request);
    }

    public updateStatus(
        repositoryName: string,
        id: string,
        status: UpdateIssueStatus,
    ): Observable<IssueStatus> {
        let request = {
            name: status.name,
            color: status.color,
        };

        return this.httpClient.put<IssueStatus>(
            `${Endpoints.Statuses(repositoryName)}/${id}`,
            request,
        );
    }

    public enableStatus(repositoryName: string, id: string): Observable<void> {
        return this.httpClient.post<void>(
            `${Endpoints.Statuses(repositoryName)}/${id}/enable`,
            null,
        );
    }

    public disableStatus(repositoryName: string, id: string): Observable<void> {
        return this.httpClient.post<void>(
            `${Endpoints.Statuses(repositoryName)}/${id}/disable`,
            null,
        );
    }

    public getStatusTransitions(repositoryName: string): Observable<IssueStatusTransition[]> {
        return this.httpClient.get<IssueStatusTransition[]>(
            Endpoints.StatusTransitions(repositoryName),
        );
    }

    public createStatusTransition(
        repositoryName: string,
        transition: CreateIssueStatusTransition,
    ): Observable<IssueStatusTransition> {
        let request = {
            fromId: transition.fromId,
            toId: transition.toId,
        };

        return this.httpClient.post<IssueStatusTransition>(
            Endpoints.StatusTransitions(repositoryName),
            request,
        );
    }

    public deleteStatusTransition(repositoryName: string, id: string): Observable<void> {
        return this.httpClient.delete<void>(`${Endpoints.StatusTransitions(repositoryName)}/${id}`);
    }
}

export interface IssueStatus {
    get id(): string;
    get name(): string;
    get color(): Color;
    get isDisabled(): boolean;
}

export interface IssueStatusTransition {
    get id(): string;
    get from(): IssueStatus;
    get to(): IssueStatus;
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
