import { loadRoles, loadRolesSuccess } from '@actions/roles.actions';
import { createReducer, on } from '@ngrx/store';
import { RolesState } from '@states/roles.state';

let initialState: RolesState = {
    data: [],
    loading: false,
};
export const rolesReducer = createReducer(
    initialState,
    on(loadRoles, (state): RolesState => ({ ...state, loading: true })),
    on(
        loadRolesSuccess,
        (state, { roles }): RolesState => ({ ...state, data: roles, loading: false }),
    ),
);
