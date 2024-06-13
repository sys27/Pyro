import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Repository } from '../models/repository';

@Injectable({
    providedIn: 'root',
})
export class RepositoryService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getRepositories(): Observable<Repository[]> {
        return this.httpClient.get<Repository[]>('/api/repositories');
    }
}
