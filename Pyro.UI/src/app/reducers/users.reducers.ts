import {
    loadCurrentUsersPageSuccess,
    loadNextUsersPageSuccess,
    loadPreviousUsersPageSuccess,
    loadUsers,
    loadUserSuccess,
    lockUserSuccess,
    unlockUserSuccess,
    usersNextPage,
    usersPreviousPage,
} from '@actions/users.actions';
import { createReducer, on } from '@ngrx/store';
import { UserItem } from '@services/user.service';
import { DataSourceState } from '@states/data-source.state';
import { UserState } from '@states/users.state';

let initialDataSource: DataSourceState<UserItem> = {
    data: [],
    loading: false,
};
let initialState: UserState = {
    previous: initialDataSource,
    current: initialDataSource,
    next: initialDataSource,
    selectedUser: null,
};

export const usersReducer = createReducer(
    initialState,
    on(loadUsers, state => ({
        ...state,
        current: { ...state.current, loading: true },
    })),
    on(loadCurrentUsersPageSuccess, (state, { users }) => ({
        ...state,
        current: {
            data: users,
            loading: false,
        },
    })),
    on(loadPreviousUsersPageSuccess, (state, { users }) => ({
        ...state,
        previous: {
            data: users,
            loading: false,
        },
    })),
    on(loadNextUsersPageSuccess, (state, { users }) => ({
        ...state,
        next: {
            data: users,
            loading: false,
        },
    })),
    on(usersPreviousPage, state => ({
        ...state,
        previous: { ...state.previous, loading: true },
        current: state.previous,
        next: state.current,
    })),
    on(usersNextPage, state => ({
        ...state,
        previous: state.current,
        current: state.next,
        next: { ...state.next, loading: true },
    })),
    on(lockUserSuccess, (state, { login }) => ({
        ...state,
        current: {
            ...state.current,
            data: state.current.data.map(user => {
                if (user.login === login) {
                    return { ...user, isLocked: true };
                }

                return user;
            }),
        },
    })),
    on(unlockUserSuccess, (state, { login }) => ({
        ...state,
        current: {
            ...state.current,
            data: state.current.data.map(user => {
                if (user.login === login) {
                    return { ...user, isLocked: false };
                }

                return user;
            }),
        },
    })),
    on(loadUserSuccess, (state, { user }) => ({
        ...state,
        selectedUser: user,
    })),
);
