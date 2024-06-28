import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, switchMap, take } from 'rxjs';
import { Endpoints } from '../endpoints';
import { CurrentUser } from '../models/current-user';

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

    private isLoginResponse(object: any): object is LoginResponse {
        return 'accessToken' in object && 'refreshToken' in object;
    }

    private isRefreshResponse(object: any): object is RefreshResponse {
        return 'accessToken' in object;
    }

    public login(login: string, password: string): Observable<CurrentUser | null> {
        return this.httpClient.post<LoginResponse>(Endpoints.Login, { login, password }).pipe(
            switchMap(response => {
                if (this.isLoginResponse(response)) {
                    localStorage.setItem(AuthService.accessTokenKey, response.accessToken);
                    localStorage.setItem(AuthService.refreshTokenKey, response.refreshToken);
                } else {
                    localStorage.removeItem(AuthService.accessTokenKey);
                    localStorage.removeItem(AuthService.refreshTokenKey);
                }

                this.updateCurrentUser();

                return this.currentUser;
            }),
            take(1),
        );
    }

    public refresh(): Observable<CurrentUser | null> {
        let refreshToken = localStorage.getItem(AuthService.refreshTokenKey);
        if (!refreshToken) {
            throw new Error('No refresh token');
        }

        return this.httpClient.post<RefreshResponse>(Endpoints.Refresh, { refreshToken }).pipe(
            switchMap(response => {
                if (this.isRefreshResponse(response)) {
                    localStorage.setItem(AuthService.accessTokenKey, response.accessToken);
                } else {
                    localStorage.removeItem(AuthService.accessTokenKey);
                    localStorage.removeItem(AuthService.refreshTokenKey);
                }

                this.updateCurrentUser();

                return this.currentUser;
            }),
            take(1),
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
