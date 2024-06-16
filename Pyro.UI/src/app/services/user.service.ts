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

    public getUser(login: string): Observable<User> {
        return this.httpClient.get<User>(`${Endpoints.Users}/${login}`);
    }

    public getRoles(): Observable<Role[]> {
        return this.httpClient.get<Role[]>(Endpoints.Roles);
    }

    public getPermissions(): Observable<Permission[]> {
        return this.httpClient.get<Permission[]>(Endpoints.Permissions);
    }

    public createUser(user: CreateUser): Observable<void> {
        let request = {
            login: user.login,
            password: user.password,
            roles: user.roles.map(role => role.name),
        };

        return this.httpClient.post<void>(Endpoints.Users, request);
    }

    public updateUser(login: string, user: UpdateUser): Observable<void> {
        let request = {
            roles: user.roles.map(role => role.name),
        };

        return this.httpClient.put<void>(`${Endpoints.Users}/${login}`, request);
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
    get login(): string;

    get isLocked(): boolean;

    get roles(): Role[];
}

export interface UserItem {
    get login(): string;
}

export interface CreateUser {
    login: string;

    password: string;

    roles: Role[];
}

export interface UpdateUser {
    roles: Role[];
}
