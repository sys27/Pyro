import { createAction, props } from '@ngrx/store';
import { CreateLabel, Label, UpdateLabel } from '@services/label.service';

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

export const updateLabel = createAction(
    '[Repository] Update Label',
    props<{ repositoryName: string; labelId: string; label: UpdateLabel }>(),
);
export const updateLabelSuccess = createAction(
    '[Repository] Update Label Success',
    props<{ label: Label }>(),
);
export const updateLabelFailure = createAction('[Repository] Update Label Failure');
