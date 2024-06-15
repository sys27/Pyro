import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';
import { User } from '../models/user';
import { UserItem } from '../models/user-item';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getUsers(): Observable<UserItem[]> {
        return this.httpClient.get<UserItem[]>(Endpoints.Users);
    }

    public getUser(email: string): Observable<User> {
        return this.httpClient.get<User>(`${Endpoints.Users}/${email}`);
    }
}
