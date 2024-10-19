import { createAction, props } from '@ngrx/store';
import { BranchItem, Repository, TreeView } from '@services/repository.service';

export const loadRepository = createAction(
    '[Repository] Load Repository',
    props<{ repositoryName: string }>(),
);
export const reloadRepository = createAction(
    '[Repository] Reload Repository',
    props<{ repositoryName: string }>(),
);
export const loadRepositorySuccess = createAction(
    '[Repository] Load Repository Success',
    props<{ repository: Repository }>(),
);
export const loadRepositoryFailure = createAction('[Repository] Load Repository Failure');

export const loadBranches = createAction(
    '[Repository] Load Repository',
    props<{ repositoryName: string }>(),
);
export const loadBranchesSuccess = createAction(
    '[Repository] Load Branches Success',
    props<{ branches: BranchItem[] }>(),
);
export const loadBranchesFailure = createAction('[Repository] Load Branches Failure');

export const setBranchOrPath = createAction(
    '[Repository] Set Branch Or Path',
    props<{ branchOrPath: string[] }>(),
);
export const setBranchOrPathSuccess = createAction(
    '[Repository] Set Branch Or Path Success',
    props<{ selectedBranch: BranchItem; branchOrPath: string[] }>(),
);

export const loadDirectoryViewSuccess = createAction(
    '[Repository] Load Directory View Success',
    props<{ directoryView: TreeView }>(),
);
export const loadDirectoryViewFailure = createAction('[Repository] Load Directory View Failure');

export const loadFileSuccess = createAction(
    '[Repository] Load File Success',
    props<{ fileName: string; content: string }>(),
);
export const loadFileFailure = createAction('[Repository] Load File Failure');
