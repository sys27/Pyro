import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, of } from 'rxjs';
import { Endpoints } from '../endpoints';
import { PyroResponse, ResponseError } from '../models/response';

@Injectable({
    providedIn: 'root',
})
export class ProfileService {
    public constructor(private readonly http: HttpClient) {}

    public getProfile(): Observable<PyroResponse<Profile>> {
        return this.http
            .get<Profile>(Endpoints.Profile)
            .pipe(catchError((error: ResponseError) => of(error)));
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
    get email(): string;
    get name(): string | null;
    get status(): string | null;
}

export interface UpdateProfile {
    name: string | null;
    status: string | null;
}
