import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

type LoginResponse = {
    accessToken: string;
    refreshToken: string;
};

@Injectable({
    providedIn: 'root'
})
export class AuthService {
    public constructor(
        private httpClient: HttpClient
    ) { }

    public login(email: string, password: string): void {
        // TODO: error handling
        this.httpClient
            .post<LoginResponse>('/api/identity/login', { email, password })
            .subscribe({
                next: response => {
                    localStorage.setItem('accessToken', response.accessToken);
                    localStorage.setItem('refreshToken', response.refreshToken);
                },
                error: error => console.error(error)
            });
    }

    public getAccessToken(): string | null {
        return localStorage.getItem('accessToken');
    }
}
