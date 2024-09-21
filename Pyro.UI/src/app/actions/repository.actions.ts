import { createAction, props } from '@ngrx/store';
import { CreateLabel, Label } from '@services/label.service';
import { BranchItem, Repository, TreeView } from '@services/repository.service';

export const loadRepositoryAndBranches = createAction(
    '[Repository] Load Repository',
    props<{ repositoryName: string }>(),
);
export const loadRepositorySuccess = createAction(
    '[Repository] Load Repository Success',
    props<{ repository: Repository }>(),
);
export const loadRepositoryFailure = createAction('[Repository] Load Repository Failure');
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

export const loadLabels = createAction(
    '[Repository] Load Labels',
    props<{ repositoryName: string }>(),
);
export const loadLabelsSuccess = createAction(
    '[Repository] Load Labels Success',
    props<{ labels: Label[] }>(),
);
export const loadLabelsFailure = createAction('[Repository] Load Labels Failure');

export const enableLabel = createAction(
    '[Repository] Enable Label',
    props<{ repositoryName: string; labelId: string }>(),
);
export const enableLabelSuccess = createAction(
    '[Repository] Enable Label Success',
    props<{ labelId: string }>(),
);
export const enableLabelFailure = createAction('[Repository] Enable Label Failure');

export const disableLabel = createAction(
    '[Repository] Disable Label',
    props<{ repositoryName: string; labelId: string }>(),
);
export const disableLabelSuccess = createAction(
    '[Repository] Disable Label Success',
    props<{ labelId: string }>(),
);
export const disableLabelFailure = createAction('[Repository] Disable Label Failure');

export const createLabel = createAction(
    '[Repository] Create Label',
    props<{ repositoryName: string; label: CreateLabel }>(),
);
export const createLabelSuccess = createAction(
    '[Repository] Create Label Success',
    props<{ label: Label }>(),
);
export const createLabelFailure = createAction('[Repository] Create Label Failure');
