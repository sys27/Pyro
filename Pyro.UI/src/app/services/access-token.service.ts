import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';

@Injectable({
    providedIn: 'root',
})
export class AccessTokenService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getAccessTokens(accessTokenName?: string): Observable<AccessToken[]> {
        let httpParams = new HttpParams();
        if (accessTokenName) {
            httpParams = httpParams.set('accessTokenName', accessTokenName);
        }

        return this.httpClient.get<AccessToken[]>(Endpoints.AccessTokens, { params: httpParams });
    }

    public createAccessToken(token: CreateAccessToken): Observable<AccessToken> {
        let request = {
            name: token.name,
            expiresAt: token.expiresAt,
        };

        return this.httpClient.post<AccessToken>(Endpoints.AccessTokens, request);
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
