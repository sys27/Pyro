import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';

@Injectable({
    providedIn: 'root',
})
export class ProfileService {
    public constructor(private http: HttpClient) {}

    public getProfile(): Observable<Profile> {
        return this.http.get<Profile>(Endpoints.Profile);
    }

    public updateProfile(profile: UpdateProfile): Observable<void> {
        let request = {
            name: profile.name,
            status: profile.status,
        };

        return this.http.put<void>(Endpoints.Profile, request);
    }
}

export interface Profile {
    get name(): string;
    get status(): string;
}

export interface UpdateProfile {
    name: string;
    status: string;
}
