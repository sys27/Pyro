import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, catchError, of } from 'rxjs';
import { Endpoints } from '../endpoints';
import { PyroResponse, ResponseError } from '../models/response';

@Injectable({
    providedIn: 'root',
})
export class RepositoryService {
    public constructor(private readonly httpClient: HttpClient) {}

    public getRepositories(): Observable<PyroResponse<RepositoryItem[]>> {
        return this.httpClient
            .get<RepositoryItem[]>(Endpoints.Repositories)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public getRepository(name: string): Observable<PyroResponse<Repository>> {
        return this.httpClient
            .get<Repository>(`${Endpoints.Repositories}/${name}`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public createRepository(repository: CreateRepository): Observable<void> {
        let request = {
            name: repository.name,
            description: repository.description,
            defaultBranch: repository.defaultBranch,
        };

        return this.httpClient.post<void>(Endpoints.Repositories, request);
    }

    public getBranches(name: string): Observable<PyroResponse<BranchItem[]>> {
        return this.httpClient
            .get<BranchItem[]>(`${Endpoints.Repositories}/${name}/branches`)
            .pipe(catchError((error: ResponseError) => of(error)));
    }

    public getTreeView(name: string, branchOrPath?: string): Observable<PyroResponse<TreeView>> {
        let observable$: Observable<TreeView>;

        if (branchOrPath) {
            observable$ = this.httpClient.get<TreeView>(
                `${Endpoints.Repositories}/${name}/tree/${branchOrPath}`,
            );
        } else {
            observable$ = this.httpClient.get<TreeView>(`${Endpoints.Repositories}/${name}/tree`);
        }

        return observable$.pipe(catchError((error: ResponseError) => of(error)));
    }

    public getFile(name: string, branchAndPath: string): Observable<PyroResponse<Blob>> {
        return this.httpClient
            .get(`${Endpoints.Repositories}/${name}/file/${branchAndPath}`, {
                responseType: 'blob',
            })
            .pipe(catchError((error: ResponseError) => of(error)));
    }
}

export interface Repository {
    get name(): string;
    get description(): string | undefined;
    get defaultBranch(): string;
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
