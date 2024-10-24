import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Endpoints } from '../endpoints';

@Injectable({
    providedIn: 'root',
})
export class RepositoryService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getRepositories(before?: string, after?: string): Observable<RepositoryItem[]> {
        let params = new HttpParams().set('size', 20);

        if (before) {
            params = params.set('before', before);
        }

        if (after) {
            params = params.set('after', after);
        }

        return this.httpClient.get<RepositoryItem[]>(Endpoints.Repositories, { params: params });
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

    public getTreeView(name: string, branchOrPath?: string): Observable<TreeView> {
        if (branchOrPath) {
            return this.httpClient.get<TreeView>(
                `${Endpoints.Repositories}/${name}/tree/${branchOrPath}`,
            );
        }

        return this.httpClient.get<TreeView>(`${Endpoints.Repositories}/${name}/tree`);
    }

    public getFile(name: string, branchAndPath: string): Observable<Blob> {
        return this.httpClient.get(`${Endpoints.Repositories}/${name}/file/${branchAndPath}`, {
            responseType: 'blob',
        });
    }
}

export interface Repository {
    get name(): string;
    get description(): string | undefined;
    get defaultBranch(): string;
    get status(): RepositoryStatus;
}

export enum RepositoryStatus {
    New = 1,
    Initialized = 2,
}

export interface RepositoryItem {
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
    get isDefault(): boolean;
}
