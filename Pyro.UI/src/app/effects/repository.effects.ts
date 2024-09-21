import { notifyAction } from '@actions/notification.actions';
import {
    createLabel,
    createLabelFailure,
    createLabelSuccess,
    disableLabel,
    disableLabelFailure,
    disableLabelSuccess,
    enableLabel,
    enableLabelFailure,
    enableLabelSuccess,
    loadBranchesFailure,
    loadBranchesSuccess,
    loadDirectoryViewFailure,
    loadDirectoryViewSuccess,
    loadFileFailure,
    loadFileSuccess,
    loadLabels,
    loadLabelsFailure,
    loadLabelsSuccess,
    loadRepositoryAndBranches,
    loadRepositoryFailure,
    loadRepositorySuccess,
    setBranchOrPath,
    setBranchOrPathSuccess,
} from '@actions/repository.actions';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { LabelService } from '@services/label.service';
import { BranchItem, RepositoryService } from '@services/repository.service';
import { AppState } from '@states/app.state';
import {
    branchOrPathSelector,
    labelsSelector,
    licenseFiles,
    readmeFiles,
    repositorySelector,
} from '@states/repository.state';
import { catchError, combineLatest, filter, map, of, switchMap, tap, withLatestFrom } from 'rxjs';

export const loadRepositoryEffect = createEffect(
    (actions$ = inject(Actions), service = inject(RepositoryService)) =>
        actions$.pipe(
            ofType(loadRepositoryAndBranches),
            switchMap(({ repositoryName }) => service.getRepository(repositoryName)),
            map(repository => loadRepositorySuccess({ repository })),
            catchError(() => of(loadRepositoryFailure())),
        ),
    { functional: true },
);

export const loadBranchesEffect = createEffect(
    (actions$ = inject(Actions), service = inject(RepositoryService)) =>
        actions$.pipe(
            ofType(loadRepositoryAndBranches),
            switchMap(({ repositoryName }) => service.getBranches(repositoryName)),
            map(branches => loadBranchesSuccess({ branches })),
            catchError(() => of(loadBranchesFailure())),
        ),
    { functional: true },
);

export const selectBranchEffect = createEffect(
    (actions$ = inject(Actions)) =>
        combineLatest([
            actions$.pipe(ofType(loadBranchesSuccess)),
            actions$.pipe(ofType(setBranchOrPath)),
        ]).pipe(
            map(([{ branches }, { branchOrPath }]) => {
                let selectedBranch = findBestBranch(branches, branchOrPath);

                return setBranchOrPathSuccess({ selectedBranch, branchOrPath });
            }),
        ),
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
    (actions$ = inject(Actions), service = inject(RepositoryService)) =>
        combineLatest([
            actions$.pipe(ofType(loadRepositorySuccess)),
            actions$.pipe(ofType(setBranchOrPath)),
        ]).pipe(
            filter(([{ repository }, _]) => !!repository),
            switchMap(([{ repository }, { branchOrPath }]) =>
                service.getTreeView(repository!.name, branchOrPath.join('/')),
            ),
            map(directoryView => loadDirectoryViewSuccess({ directoryView })),
            catchError(() => of(loadDirectoryViewFailure())),
        ),
    { functional: true },
);

// TODO: load files only after UI in view
export const loadReadmeFileEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) =>
        actions$.pipe(
            ofType(loadDirectoryViewSuccess),
            withLatestFrom(store.select(repositorySelector), store.select(branchOrPathSelector)),
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
        ),
    { functional: true },
);

// TODO: load files only after UI in view
export const loadLicenseFileEffect = createEffect(
    (
        actions$ = inject(Actions),
        store = inject(Store<AppState>),
        service = inject(RepositoryService),
    ) =>
        actions$.pipe(
            ofType(loadDirectoryViewSuccess),
            withLatestFrom(store.select(repositorySelector), store.select(branchOrPathSelector)),
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
        ),
    { functional: true },
);

export const loadLabelsEffect = createEffect(
    (actions$ = inject(Actions), store = inject(Store<AppState>), service = inject(LabelService)) =>
        actions$.pipe(
            ofType(loadLabels),
            withLatestFrom(store.select(labelsSelector)),
            filter(([_, labels]) => labels.loading),
            switchMap(([{ repositoryName }]) => service.getLabels(repositoryName)),
            map(labels => loadLabelsSuccess({ labels })),
            catchError(() => of(loadLabelsFailure())),
        ),
    { functional: true },
);

export const enableLabelEffect = createEffect(
    (actions$ = inject(Actions), service = inject(LabelService)) =>
        actions$.pipe(
            ofType(enableLabel),
            switchMap(({ repositoryName, labelId }) =>
                service.enableLabel(repositoryName, labelId).pipe(map(() => labelId)),
            ),
            map(labelId => enableLabelSuccess({ labelId })),
            catchError(() => of(enableLabelFailure())),
        ),
    { functional: true },
);

export const enableLabelSuccessEffect = createEffect(
    (actions$ = inject(Actions)) =>
        actions$.pipe(
            ofType(enableLabelSuccess),
            map(() =>
                notifyAction({
                    severity: 'success',
                    title: 'Success',
                    message: 'Label enabled',
                }),
            ),
        ),
    { functional: true },
);

export const disableLabelEffect = createEffect(
    (actions$ = inject(Actions), service = inject(LabelService)) =>
        actions$.pipe(
            ofType(disableLabel),
            switchMap(({ repositoryName, labelId }) =>
                service.disableLabel(repositoryName, labelId).pipe(map(() => labelId)),
            ),
            map(labelId => disableLabelSuccess({ labelId })),
            catchError(() => of(disableLabelFailure())),
        ),
    { functional: true },
);

export const disableLabelSuccessEffect = createEffect(
    (actions$ = inject(Actions)) =>
        actions$.pipe(
            ofType(disableLabelSuccess),
            map(() =>
                notifyAction({
                    severity: 'success',
                    title: 'Success',
                    message: 'Label disabled',
                }),
            ),
        ),
    { functional: true },
);

export const createLabelEffect = createEffect(
    (actions$ = inject(Actions), service = inject(LabelService)) =>
        actions$.pipe(
            ofType(createLabel),
            switchMap(({ repositoryName, label }) => service.createLabel(repositoryName, label)),
            map(label => createLabelSuccess({ label })),
            catchError(() => of(createLabelFailure())),
        ),
    { functional: true },
);

export const createLabelSuccessEffect = createEffect(
    (actions$ = inject(Actions), router = inject(Router), store = inject(Store<AppState>)) =>
        actions$.pipe(
            ofType(createLabelSuccess),
            withLatestFrom(store.select(repositorySelector)),
            filter(([_, repository]) => !!repository),
            tap(([_, repository]) =>
                router.navigate(['repositories', repository!.name, 'settings', 'labels']),
            ),
        ),
    { functional: true, dispatch: false },
);
