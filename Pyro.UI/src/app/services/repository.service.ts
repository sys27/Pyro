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
            description: repository.description,
            defaultBranch: repository.defaultBranch,
        };

        return this.httpClient.post<void>(Endpoints.Repositories, request);
    }

    public getBranches(name: string): Observable<BranchItem[]> {
        return this.httpClient.get<BranchItem[]>(`${Endpoints.Repositories}/${name}/branches`);
    }

    public getTreeView(name: string, branchOrHash?: string, path?: string): Observable<TreeView> {
        if (branchOrHash && path) {
            return this.httpClient.get<TreeView>(
                `${Endpoints.Repositories}/${name}/tree/${branchOrHash}/${path}`,
            );
        }

        if (branchOrHash) {
            return this.httpClient.get<TreeView>(
                `${Endpoints.Repositories}/${name}/tree/${branchOrHash}`,
            );
        }

        return this.httpClient.get<TreeView>(`${Endpoints.Repositories}/${name}/tree`);
    }
}

export interface Repository {
    get name(): string;
    get description(): string | undefined;
    get defaultBranch(): string;
}

export interface CreateRepository {
    name: string;
    description?: string;
    defaultBranch: string;
}

export interface TreeView {
    get commit(): CommitInfo;
    get items(): TreeViewItem[];
    get commitsCount(): number;
}

export interface TreeViewItem {
    get name(): string;
    get isDirectory(): boolean;
    get message(): string;
    get date(): Date;
}

export interface CommitInfo {
    get hash(): string;
    get author(): CommitUser;
    get message(): string;
    get date(): Date;
}

export interface CommitUser {
    get name(): string;
    get email(): string;
}

export interface BranchItem {
    get name(): string;
    get lastCommit(): CommitInfo;
}
