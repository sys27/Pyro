import { loggedInAction, loggedOutAction, refreshedAction } from '@actions/auth.actions';
import { CurrentUser } from '@models/current-user';
import { ActionReducer, createReducer, INIT, on } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { AuthState } from '@states/auth.state';

let initialState: AuthState = {
    currentUser: null,
};

export const authReducer = createReducer(
    initialState,
    on(
        loggedInAction,
        (state, { currentUser }): AuthState => ({
            ...state,
            currentUser,
        }),
    ),
    on(
        loggedOutAction,
        (state): AuthState => ({
            ...state,

            currentUser: null,
        }),
    ),
    on(
        refreshedAction,
        (state, { currentUser }): AuthState => ({
            ...state,
            currentUser,
        }),
    ),
);

const localStorageKey = 'auth';

// TODO: ???
export function saveStateReducer(reducer: ActionReducer<AppState>): ActionReducer<AppState> {
    return (state, action) => {
        let result = reducer(state, action);

        if (action.type === INIT) {
            let stored = localStorage.getItem(localStorageKey);
            if (stored) {
                let auth = JSON.parse(stored);
                if (!auth.currentUser) {
                    result = {
                        ...result,
                        auth: initialState,
                    };
                } else {
                    result = {
                        ...result,
                        auth: {
                            currentUser: new CurrentUser(
                                auth.currentUser._accessToken,
                                auth.currentUser._refreshToken,
                                new Date(auth.currentUser._expiresIn),
                                auth.currentUser._id,
                                auth.currentUser._login,
                                auth.currentUser._roles,
                                auth.currentUser._permissions,
                            ),
                        },
                    };
                }
            }
        } else if (action.type === loggedInAction.type || action.type === refreshedAction.type) {
            localStorage.setItem(localStorageKey, JSON.stringify(result.auth));
        } else if (action.type === loggedOutAction.type) {
            localStorage.removeItem(localStorageKey);
        }

        return result;
    };
}
