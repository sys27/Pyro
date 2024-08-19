import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Color } from '@models/color';
import { catchError, Observable, of } from 'rxjs';
import { Endpoints } from '../endpoints';
import { PyroResponse, ResponseError } from '../models/response';

@Injectable({ providedIn: 'root' })
export class LabelService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getLabels(repositoryName: string): Observable<PyroResponse<Label[]>> {
        return this.httpClient
            .get<Label[]>(Endpoints.Labels(repositoryName))
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public getLabel(repositoryName: string, id: string): Observable<PyroResponse<Label>> {
        return this.httpClient
            .get<Label>(`${Endpoints.Labels(repositoryName)}/${id}`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public createLabel(repositoryName: string, label: CreateLabel): Observable<PyroResponse<Label>> {
        let request = {
            name: label.name,
            color: label.color,
        };

        return this.httpClient
            .post<Label>(Endpoints.Labels(repositoryName), request)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public updateLabel(
        repositoryName: string,
        id: string,
        label: UpdateLabel,
    ): Observable<PyroResponse<Label>> {
        let request = {
            name: label.name,
            color: label.color,
        };

        return this.httpClient
            .put<Label>(`${Endpoints.Labels(repositoryName)}/${id}`, request)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public deleteLabel(repositoryName: string, id: string): Observable<PyroResponse<void>> {
        return this.httpClient
            .delete<void>(`${Endpoints.Labels(repositoryName)}/${id}`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }
}

export interface Label {
    id: string;
    name: string;
    color: Color;
}

export interface CreateLabel {
    name: string;
    color: Color;
}

export interface UpdateLabel {
    name: string;
    color: Color;
}
