import { CurrentUser } from '@models/current-user';
import { createAction, props } from '@ngrx/store';

export const loginAction = createAction(
    '[Auth] Login',
    props<{ login: string; password: string; returnUrl: string }>(),
);
export const loggedInAction = createAction(
    '[Auth] Logged In',
    props<{ currentUser: CurrentUser; returnUrl: string }>(),
);
export const loginFailedAction = createAction('[Auth] Login Failed');

export const logoutAction = createAction('[Auth] Logout');
export const loggedOutAction = createAction('[Auth] Logged Out');
export const logoutFailedAction = createAction('[Auth] Logout Failed');

export const refreshAction = createAction('[Auth] Refresh', props<{ refreshToken: string }>());
export const refreshedAction = createAction(
    '[Auth] Refreshed',
    props<{ currentUser: CurrentUser }>(),
);
export const refreshFailedAction = createAction('[Auth] Refresh Failed');
