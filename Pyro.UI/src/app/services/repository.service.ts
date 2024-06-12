import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Repository } from '../models/Repository';

@Injectable({
    providedIn: 'root'
})
export class RepositoryService {
    public constructor(
        private httpClient: HttpClient
    ) { }

    public getRepositories(): Observable<Repository[]> {
        return this.httpClient.get<Repository[]>('/api/repositories');
    }
}
