import {
    createIssue,
    createIssueFailure,
    createIssueSuccess,
    loadIssueUsers,
    loadIssueUsersFailure,
    loadIssueUsersSuccess,
    newIssueComponentOpened,
} from '@actions/issues.actions';
import { loadLabels } from '@actions/repository-labels.actions';
import { loadStatuses } from '@actions/repository-statuses.actions';
import {
    loadBranches,
    loadBranchesFailure,
    loadBranchesSuccess,
    loadDirectoryViewFailure,
    loadDirectoryViewSuccess,
    loadFileFailure,
    loadFileSuccess,
    loadRepository,
    loadRepositoryFailure,
    loadRepositorySuccess,
    reloadRepository,
    setBranchOrPath,
    setBranchOrPathSuccess,
} from '@actions/repository.actions';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { concatLatestFrom } from '@ngrx/operators';
import { Store } from '@ngrx/store';
import { IssueService } from '@services/issue.service';
import { BranchItem, RepositoryService } from '@services/repository.service';
import { AppState } from '@states/app.state';
import {
    licenseFiles,
    readmeFiles,
    selectBranches,
    selectBranchOrPath,
    selectRepository,
} from '@states/repository.state';
import { selectRouteParam } from '@states/router.state';
import { catchError, combineLatest, filter, map, of, switchMap, tap } from 'rxjs';

export const loadRepositoryEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) => {
        return actions$.pipe(
            ofType(loadRepository),
            concatLatestFrom(() => store.select(selectRepository)),
            filter(
                ([{ repositoryName }, repository]) =>
                    !repository || repository.name !== repositoryName,
            ),
            switchMap(([{ repositoryName }]) => service.getRepository(repositoryName)),
            map(repository => loadRepositorySuccess({ repository })),
            catchError(() => of(loadRepositoryFailure())),
        );
    },
    { functional: true },
);

export const reloadRepositoryEffect = createEffect(
    (actions$ = inject(Actions), service = inject(RepositoryService)) => {
        return actions$.pipe(
            ofType(reloadRepository),
            switchMap(({ repositoryName }) => service.getRepository(repositoryName)),
            map(repository => loadRepositorySuccess({ repository })),
            catchError(() => of(loadRepositoryFailure())),
        );
    },
    { functional: true },
);

export const loadBranchesEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) => {
        return actions$.pipe(
            ofType(loadBranches),
            concatLatestFrom(() => store.select(selectBranches)),
            filter(([_, branches]) => branches.loading),
            switchMap(([{ repositoryName }]) => service.getBranches(repositoryName)),
            map(branches => loadBranchesSuccess({ branches })),
            catchError(() => of(loadBranchesFailure())),
        );
    },
    { functional: true },
);

export const selectBranchEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return combineLatest([
            actions$.pipe(ofType(loadBranchesSuccess)),
            actions$.pipe(ofType(setBranchOrPath)),
        ]).pipe(
            map(([{ branches }, { branchOrPath }]) => {
                let selectedBranch = findBestBranch(branches, branchOrPath);

                return setBranchOrPathSuccess({ selectedBranch, branchOrPath });
            }),
        );
    },
    { functional: true },
);

function findBestBranch(branches: BranchItem[], branchOrPath: string[]): BranchItem {
    let end = 0;
    while (end < branchOrPath.length) {
        let branchName = branchOrPath.slice(0, end + 1).join('/');
        let branch = branches.find(b => b.name === branchName);
        if (branch != null) {
            return branch;
        }

        end++;
    }

    let defaultBranch = branches.find(b => b.isDefault);

    return defaultBranch ?? branches[0];
}

export const loadDirectoryViewEffect = createEffect(
    (actions$ = inject(Actions), service = inject(RepositoryService)) => {
        return combineLatest([
            actions$.pipe(ofType(loadRepositorySuccess)),
            actions$.pipe(ofType(setBranchOrPath)),
        ]).pipe(
            filter(([{ repository }, _]) => !!repository),
            switchMap(([{ repository }, { branchOrPath }]) =>
                service.getTreeView(repository!.name, branchOrPath.join('/')),
            ),
            map(directoryView => loadDirectoryViewSuccess({ directoryView })),
            catchError(() => of(loadDirectoryViewFailure())),
        );
    },
    { functional: true },
);

// TODO: load files only after UI in view
export const loadReadmeFileEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) => {
        return actions$.pipe(
            ofType(loadDirectoryViewSuccess),
            concatLatestFrom(() => [
                store.select(selectRepository),
                store.select(selectBranchOrPath),
            ]),
            filter(([_, repository]) => !!repository),
            map(([{ directoryView }, repository, branchOrPath]) => ({
                file: directoryView.items.find(f => readmeFiles.includes(f.name.toLowerCase())),
                repository,
                branchOrPath,
            })),
            filter(({ file }) => !!file),
            switchMap(({ file, repository, branchOrPath }) => {
                let fileName = file!.name;
                let path = branchOrPath.concat(fileName).join('/');

                return service.getFile(repository!.name, path).pipe(
                    switchMap(blob => blob.text()),
                    map(blob => ({ fileName, blob })),
                );
            }),
            map(({ fileName, blob }) => loadFileSuccess({ fileName, content: blob })),
            catchError(() => of(loadFileFailure())),
        );
    },
    { functional: true },
);

// TODO: load files only after UI in view
export const loadLicenseFileEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) => {
        return actions$.pipe(
            ofType(loadDirectoryViewSuccess),
            concatLatestFrom(() => [
                store.select(selectRepository),
                store.select(selectBranchOrPath),
            ]),
            filter(([_, repository]) => !!repository),
            map(([{ directoryView }, repository, branchOrPath]) => ({
                file: directoryView.items.find(f => licenseFiles.includes(f.name.toLowerCase())),
                repository,
                branchOrPath,
            })),
            filter(({ file }) => !!file),
            switchMap(({ file, repository, branchOrPath }) => {
                let fileName = file!.name;
                let path = branchOrPath.concat(fileName).join('/');

                return service.getFile(repository!.name, path).pipe(
                    switchMap(blob => blob.text()),
                    map(blob => ({ fileName, blob })),
                );
            }),
            map(({ fileName, blob }) => loadFileSuccess({ fileName, content: blob })),
            catchError(() => of(loadFileFailure())),
        );
    },
    { functional: true },
);

export const loadLabelsForNewIssueComponentOpenedEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(newIssueComponentOpened),
            map(({ repositoryName }) => loadLabels({ repositoryName })),
        );
    },
    { functional: true },
);

export const loadStatusesForNewIssueComponentOpenedEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(newIssueComponentOpened),
            map(({ repositoryName }) => loadStatuses({ repositoryName })),
        );
    },
    { functional: true },
);

export const loadUsersForNewIssueComponentOpenedEffect = createEffect(
    (actions$ = inject(Actions)) => {
        return actions$.pipe(
            ofType(newIssueComponentOpened),
            map(() => loadIssueUsers()),
        );
    },
    { functional: true },
);

export const loadUsersEffect = createEffect(
    (actions$ = inject(Actions), service = inject(IssueService)) => {
        return actions$.pipe(
            ofType(loadIssueUsers),
            switchMap(() => service.getUsers()),
            map(users => loadIssueUsersSuccess({ users })),
            catchError(() => of(loadIssueUsersFailure())),
        );
    },
    { functional: true },
);

export const createIssueEffect = createEffect(
    (actions$ = inject(Actions), service = inject(IssueService)) => {
        return actions$.pipe(
            ofType(createIssue),
            switchMap(({ repositoryName, issue }) => service.createIssue(repositoryName, issue)),
            map(() => createIssueSuccess()),
            catchError(() => of(createIssueFailure())),
        );
    },
    { functional: true },
);

export const createIssueSuccessEffect = createEffect(
    (actions$ = inject(Actions), router = inject(Router), store = inject(Store<AppState>)) => {
        return actions$.pipe(
            ofType(createIssueSuccess),
            concatLatestFrom(() => store.select(selectRouteParam('repositoryName'))),
            tap(([_, repositoryName]) =>
                router.navigate(['repositories', repositoryName, 'issues']),
            ),
        );
    },
    { functional: true, dispatch: false },
);
