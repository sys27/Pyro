import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, Observable, of } from 'rxjs';
import { Endpoints } from '../endpoints';
import { PyroResponse, ResponseError } from '../models/response';

@Injectable({
    providedIn: 'root',
})
export class AccessTokenService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getAccessTokens(): Observable<PyroResponse<AccessToken[]>> {
        return this.httpClient
            .get<AccessToken[]>(Endpoints.AccessTokens)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public createAccessToken(token: CreateAccessToken): Observable<PyroResponse<AccessToken>> {
        let request = {
            name: token.name,
            expiresAt: token.expiresAt,
        };

        return this.httpClient
            .post<AccessToken>(Endpoints.AccessTokens, request)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public deleteAccessToken(tokenName: string): Observable<void> {
        return this.httpClient.delete<void>(`${Endpoints.AccessTokens}/${tokenName}`);
    }
}

export interface AccessToken {
    get name(): string;
    get expiresAt(): Date;
    get token(): string;
}

export interface CreateAccessToken {
    name: string;
    expiresAt: Date;
}
