import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, switchMap } from 'rxjs';
import { Endpoints } from '../endpoints';
import { CurrentUser } from '../models/current-user';
import { Response } from '../models/response';

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

        this.currentUser.subscribe(currentUser => console.log(currentUser));
    }

    private isLoginResponse(object: any): object is LoginResponse {
        return 'accessToken' in object && 'refreshToken' in object;
    }

    public login(email: string, password: string): Observable<CurrentUser | null> {
        return this.httpClient
            .post<Response<LoginResponse>>(Endpoints.Login, { email, password })
            .pipe(
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
            parsedPayload.sub,
            parsedPayload.email,
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
