import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';

@Injectable({
    providedIn: 'root',
})
export class ProfileService {
    public constructor(private readonly http: HttpClient) {}

    public getProfile(): Observable<Profile> {
        return this.http.get<Profile>(Endpoints.Profile);
    }

    public updateProfile(profile: UpdateProfile): Observable<void> {
        let request = {
            displayName: profile.displayName,
            email: profile.email,
        };

        return this.http.put<void>(Endpoints.Profile, request);
    }
}

export interface Profile {
    get displayName(): string;
    get email(): string;
}

export interface UpdateProfile {
    displayName: string;
    email: string;
}
