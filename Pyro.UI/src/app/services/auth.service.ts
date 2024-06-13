import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, map, switchMap } from 'rxjs';
import { CurrentUser } from '../models/current-user';

type LoginResponse = {
    accessToken: string;
    refreshToken: string;
};

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    private readonly currentUserSubject: BehaviorSubject<CurrentUser | null> = new BehaviorSubject<CurrentUser | null>(null);

    public constructor(
        private httpClient: HttpClient
    ) {
        this.updateCurrentUser();

        this.currentUser.subscribe(currentUser => console.log(currentUser));
    }

    public login(email: string, password: string): Observable<CurrentUser | null> {
        // TODO: error handling
        return this.httpClient
            .post<LoginResponse>('/api/identity/login', { email, password })
            .pipe(
                switchMap(response => {
                    localStorage.setItem('accessToken', response.accessToken);
                    localStorage.setItem('refreshToken', response.refreshToken);

                    this.updateCurrentUser();

                    return this.currentUser;
                })
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
            parsedPayload.permissions
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
        return localStorage.getItem('accessToken');
    }

    public get currentUser(): Observable<CurrentUser | null> {
        return this.currentUserSubject.asObservable();
    }
}
