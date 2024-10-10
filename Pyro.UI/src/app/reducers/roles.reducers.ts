import { loadRoles, loadRolesSuccess } from '@actions/roles.actions';
import { createReducer, on } from '@ngrx/store';
import { RolesState } from '@states/roles.state';

let initialState: RolesState = {
    data: [],
    loading: false,
};
export const rolesReducer = createReducer(
    initialState,
    on(loadRoles, state => ({ ...state, loading: true })),
    on(loadRolesSuccess, (state, { roles }) => ({ ...state, data: roles, loading: false })),
);
