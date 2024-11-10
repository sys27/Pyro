import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';

@Injectable({
    providedIn: 'root',
})
export class UserService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getUsers(before?: string, after?: string): Observable<UserItem[]> {
        let params = new HttpParams().set('size', 20);

        if (before) {
            params = params.set('before', before);
        }

        if (after) {
            params = params.set('after', after);
        }

        return this.httpClient.get<UserItem[]>(Endpoints.Users, { params: params });
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

    public lockUser(login: string): Observable<void> {
        return this.httpClient.post<void>(`${Endpoints.Users}/${login}/lock`, null);
    }

    public unlockUser(login: string): Observable<void> {
        return this.httpClient.post<void>(`${Endpoints.Users}/${login}/unlock`, null);
    }

    public activate(command: ActivateUser): Observable<void> {
        return this.httpClient.post<void>(`${Endpoints.Users}/activate`, command);
    }

    public changePassword(command: ChangePassword): Observable<void> {
        return this.httpClient.post<void>(`${Endpoints.Users}/change-password`, command);
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
    get isLocked(): boolean;
}

export interface CreateUser {
    login: string;
    roles: Role[];
}

export interface UpdateUser {
    roles: Role[];
}

export interface ActivateUser {
    get token(): string;
    get password(): string;
}

export interface ChangePassword {
    get oldPassword(): string;
    get newPassword(): string;
}
