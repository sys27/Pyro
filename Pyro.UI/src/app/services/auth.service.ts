import { HttpClient, HttpContext } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { CurrentUser } from '@models/current-user';
import { Observable, map } from 'rxjs';
import { Endpoints } from '../endpoints';
import { ALLOW_ANONYMOUS } from './auth.interceptor';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    public constructor(private readonly httpClient: HttpClient) {}

    public login(login: string, password: string): Observable<CurrentUser> {
        return this.httpClient.post<LoginResponse>(Endpoints.Login, { login, password }).pipe(
            map(response => {
                let currentUser = this.getUserFromJwt(response.accessToken, response.refreshToken);

                return currentUser;
            }),
        );
    }

    public logout(): Observable<void> {
        return this.httpClient.post<void>(Endpoints.Logout, {});
    }

    public refresh(refreshToken: string): Observable<CurrentUser> {
        return this.httpClient
            .post<RefreshResponse>(
                Endpoints.Refresh,
                { refreshToken },
                { context: new HttpContext().set(ALLOW_ANONYMOUS, true) },
            )
            .pipe(
                map(response => {
                    let currentUser = this.getUserFromJwt(response.accessToken, refreshToken);

                    return currentUser;
                }),
            );
    }

    private getUserFromJwt(accessToken: string, refreshToken: string): CurrentUser {
        let parts = accessToken.split('.');
        if (parts.length !== 3) {
            throw new Error('Invalid JWT');
        }

        let payload = parts[1];
        let decodedPayload = atob(payload);
        let parsedPayload = JSON.parse(decodedPayload);

        return new CurrentUser(
            accessToken,
            refreshToken,
            new Date(parsedPayload.exp * 1000),
            parsedPayload.sub,
            parsedPayload.login,
            parsedPayload.roles,
            parsedPayload.permissions,
        );
    }
}

interface LoginResponse {
    accessToken: string;
    refreshToken: string;
}

interface RefreshResponse {
    accessToken: string;
}
