import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, catchError, of, switchMap } from 'rxjs';
import { Endpoints } from '../endpoints';
import { CurrentUser } from '../models/current-user';
import { ResponseError } from '../models/response';

@Injectable({
    providedIn: 'root',
})
export class AuthService {
    private static readonly accessTokenKey: string = 'accessToken';
    private static readonly refreshTokenKey: string = 'refreshToken';

    private readonly currentUserSubject: BehaviorSubject<CurrentUser | null> =
        new BehaviorSubject<CurrentUser | null>(null);

    public constructor(private httpClient: HttpClient) {
        this.updateCurrentUser();
    }

    public login(login: string, password: string): Observable<CurrentUser | null> {
        return this.httpClient.post<LoginResponse>(Endpoints.Login, { login, password }).pipe(
            catchError((error: ResponseError) => {
                localStorage.removeItem(AuthService.accessTokenKey);
                localStorage.removeItem(AuthService.refreshTokenKey);
                this.updateCurrentUser();

                return of(error);
            }),
            switchMap(response => {
                if (response instanceof ResponseError) {
                    return this.currentUser;
                }

                localStorage.setItem(AuthService.accessTokenKey, response.accessToken);
                localStorage.setItem(AuthService.refreshTokenKey, response.refreshToken);
                this.updateCurrentUser();

                return this.currentUser;
            }),
        );
    }

    public refresh(): Observable<CurrentUser | null> {
        let refreshToken = localStorage.getItem(AuthService.refreshTokenKey);
        if (!refreshToken) {
            throw new Error('No refresh token');
        }

        return this.httpClient.post<RefreshResponse>(Endpoints.Refresh, { refreshToken }).pipe(
            catchError((error: ResponseError) => {
                localStorage.removeItem(AuthService.accessTokenKey);
                localStorage.removeItem(AuthService.refreshTokenKey);
                this.updateCurrentUser();

                return of(error);
            }),
            switchMap(response => {
                if (response instanceof ResponseError) {
                    return this.currentUser;
                }

                localStorage.setItem(AuthService.accessTokenKey, response.accessToken);
                this.updateCurrentUser();

                return this.currentUser;
            }),
        );
    }

    private getUserFromJwt(jwt: string): CurrentUser {
        let parts = jwt.split('.');
        if (parts.length !== 3) {
            throw new Error('Invalid JWT');
        }

        let payload = parts[1];
        let decodedPayload = atob(payload);
        let parsedPayload = JSON.parse(decodedPayload);

        return new CurrentUser(
            jwt,
            new Date(parsedPayload.exp * 1000),
            parsedPayload.sub,
            parsedPayload.login,
            parsedPayload.roles,
            parsedPayload.permissions,
        );
    }

    private updateCurrentUser(): void {
        let accessToken = this.getAccessToken();
        if (accessToken) {
            let currentUser = this.getUserFromJwt(accessToken);
            this.currentUserSubject.next(currentUser);
        } else {
            this.currentUserSubject.next(null);
        }
    }

    public getAccessToken(): string | null {
        return localStorage.getItem(AuthService.accessTokenKey);
    }

    public get currentUser(): Observable<CurrentUser | null> {
        return this.currentUserSubject.asObservable();
    }
}

interface LoginResponse {
    accessToken: string;
    refreshToken: string;
}

interface RefreshResponse {
    accessToken: string;
}
