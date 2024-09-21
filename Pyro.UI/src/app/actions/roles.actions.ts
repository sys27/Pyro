import { createAction, props } from '@ngrx/store';
import { Role } from '@services/user.service';

export const loadRoles = createAction('[Roles] Load Roles');
export const loadRolesSuccess = createAction(
    '[Roles] Load Roles Success',
    props<{ roles: Role[] }>(),
);
export const loadRolesFailure = createAction('[Roles] Load Roles Failure');
