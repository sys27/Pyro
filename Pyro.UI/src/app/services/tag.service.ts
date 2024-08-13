import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Color } from '@models/color';
import { catchError, Observable, of } from 'rxjs';
import { Endpoints } from '../endpoints';
import { PyroResponse, ResponseError } from '../models/response';

@Injectable({ providedIn: 'root' })
export class TagService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getTags(repositoryName: string): Observable<PyroResponse<Tag[]>> {
        return this.httpClient
            .get<Tag[]>(Endpoints.Tags(repositoryName))
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public getTag(repositoryName: string, id: string): Observable<PyroResponse<Tag>> {
        return this.httpClient
            .get<Tag>(`${Endpoints.Tags(repositoryName)}/${id}`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public createTag(repositoryName: string, tag: CreateTag): Observable<PyroResponse<Tag>> {
        let request = {
            name: tag.name,
            color: tag.color,
        };

        return this.httpClient
            .post<Tag>(Endpoints.Tags(repositoryName), request)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public updateTag(
        repositoryName: string,
        id: string,
        tag: UpdateTag,
    ): Observable<PyroResponse<Tag>> {
        let request = {
            name: tag.name,
            color: tag.color,
        };

        return this.httpClient
            .put<Tag>(`${Endpoints.Tags(repositoryName)}/${id}`, request)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public deleteTag(repositoryName: string, id: string): Observable<PyroResponse<void>> {
        return this.httpClient
            .delete<void>(`${Endpoints.Tags(repositoryName)}/${id}`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }
}

export interface Tag {
    id: string;
    name: string;
    color: Color;
}

export interface CreateTag {
    name: string;
    color: Color;
}

export interface UpdateTag {
    name: string;
    color: Color;
}
