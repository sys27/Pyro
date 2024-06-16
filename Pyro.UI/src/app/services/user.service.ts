import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';

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

    public getRoles(): Observable<Role[]> {
        return this.httpClient.get<Role[]>(Endpoints.Roles);
    }

    public getPermissions(): Observable<Permission[]> {
        return this.httpClient.get<Permission[]>(Endpoints.Permissions);
    }

    public createUser(user: CreateUser): Observable<void> {
        let request = {
            email: user.email,
            password: user.password,
            roles: user.roles.map(role => role.name),
        };

        return this.httpClient.post<void>(Endpoints.Users, request);
    }

    public updateUser(email: string, user: UpdateUser): Observable<void> {
        let request = {
            roles: user.roles.map(role => role.name),
        };

        return this.httpClient.put<void>(`${Endpoints.Users}/${email}`, request);
    }
}

export interface Permission {
    get name(): string;
}

export interface Role {
    get name(): string;
    get permissions(): Permission[];
}

export interface User {
    get email(): string;

    get isLocked(): boolean;

    get roles(): Role[];
}

export interface UserItem {
    get email(): string;
}

export interface CreateUser {
    email: string;

    password: string;

    roles: Role[];
}

export interface UpdateUser {
    roles: Role[];
}
