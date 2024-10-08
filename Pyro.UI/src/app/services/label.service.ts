import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Color } from '@models/color';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';

@Injectable({ providedIn: 'root' })
export class LabelService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getLabels(repositoryName: string, labelName?: string): Observable<Label[]> {
        let httpParams = new HttpParams();
        if (labelName) {
            httpParams = httpParams.set('labelName', labelName);
        }

        return this.httpClient.get<Label[]>(Endpoints.Labels(repositoryName), {
            params: httpParams,
        });
    }

    public getLabel(repositoryName: string, id: string): Observable<Label> {
        return this.httpClient.get<Label>(`${Endpoints.Labels(repositoryName)}/${id}`);
    }

    public createLabel(repositoryName: string, label: CreateLabel): Observable<Label> {
        let request = {
            name: label.name,
            color: label.color,
        };

        return this.httpClient.post<Label>(Endpoints.Labels(repositoryName), request);
    }

    public updateLabel(repositoryName: string, id: string, label: UpdateLabel): Observable<Label> {
        let request = {
            name: label.name,
            color: label.color,
        };

        return this.httpClient.put<Label>(`${Endpoints.Labels(repositoryName)}/${id}`, request);
    }

    public enableLabel(repositoryName: string, id: string): Observable<void> {
        return this.httpClient.post<void>(`${Endpoints.Labels(repositoryName)}/${id}/enable`, null);
    }

    public disableLabel(repositoryName: string, id: string): Observable<void> {
        return this.httpClient.post<void>(
            `${Endpoints.Labels(repositoryName)}/${id}/disable`,
            null,
        );
    }
}

export interface Label {
    get id(): string;
    get name(): string;
    get color(): Color;
    get isDisabled(): boolean;
}

export interface CreateLabel {
    name: string;
    color: Color;
}

export interface UpdateLabel {
    name: string;
    color: Color;
}
