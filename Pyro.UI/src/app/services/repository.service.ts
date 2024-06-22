import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';
import { RepositoryItem } from '../models/repository-item';

@Injectable({
    providedIn: 'root',
})
export class RepositoryService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getRepositories(): Observable<RepositoryItem[]> {
        return this.httpClient.get<RepositoryItem[]>(Endpoints.Repositories);
    }

    public getRepository(name: string): Observable<Repository> {
        return this.httpClient.get<Repository>(`${Endpoints.Repositories}/${name}`);
    }

    public createRepository(repository: CreateRepository): Observable<void> {
        let request = {
            name: repository.name,
        };

        return this.httpClient.post<void>(Endpoints.Repositories, request);
    }
}

export interface Repository {
    get name(): string;
}

export interface CreateRepository {
    name: string;
}
