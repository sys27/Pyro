import { createAction, props } from '@ngrx/store';
import { CreateUser, UpdateUser, User, UserItem } from '@services/user.service';

export const loadUsers = createAction('[Users] Load Users');
export const loadCurrentUsersPageSuccess = createAction(
    '[Users] Load Current Users Page Success',
    props<{ users: UserItem[] }>(),
);
export const loadPreviousUsersPageSuccess = createAction(
    '[Users] Load Previous Users Page Success',
    props<{ users: UserItem[] }>(),
);
export const loadNextUsersPageSuccess = createAction(
    '[Users] Load Next Users Page Success',
    props<{ users: UserItem[] }>(),
);
export const loadUsersFailure = createAction('[Users] Load Users Failure');
export const usersPreviousPage = createAction('[Users] Previous Page');
export const usersNextPage = createAction('[Users] Next Page');

export const lockUser = createAction('[Users] Lock User', props<{ login: string }>());
export const lockUserSuccess = createAction(
    '[Users] Lock User Success',
    props<{ login: string }>(),
);
export const lockUserFailure = createAction('[Users] Lock User Failure');
export const unlockUser = createAction('[Users] Unlock User', props<{ login: string }>());
export const unlockUserSuccess = createAction(
    '[Users] Unlock User Success',
    props<{ login: string }>(),
);
export const unlockUserFailure = createAction('[Users] Unlock User Failure');

export const createUser = createAction('[Users] Create User', props<{ user: CreateUser }>());
export const createUserSuccess = createAction(
    '[Users] Create User Success',
    props<{ login: string }>(),
);
export const createUserFailure = createAction('[Users] Create User Failure');

export const updateUser = createAction(
    '[Users] Update User',
    props<{ login: string; user: UpdateUser }>(),
);
export const updateUserSuccess = createAction(
    '[Users] Update User Success',
    props<{ login: string }>(),
);
export const updateUserFailure = createAction('[Users] Update User Failure');

export const loadUser = createAction('[Users] Load User', props<{ login: string }>());
export const loadUserSuccess = createAction('[Users] Load User Success', props<{ user: User }>());
export const loadUserFailure = createAction('[Users] Load User Failure');

export const changePassword = createAction(
    '[Users] Change Password',
    props<{ oldPassword: string; newPassword: string }>(),
);
export const changePasswordSuccess = createAction('[Users] Change Password Success');
export const changePasswordFailure = createAction('[Users] Change Password Failure');
