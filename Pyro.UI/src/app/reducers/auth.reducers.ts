import { loggedInAction, loggedOutAction, refreshedAction } from '@actions/auth.actions';
import { CurrentUser } from '@models/current-user';
import { ActionReducer, createReducer, on } from '@ngrx/store';
import { AuthState } from '@states/auth.state';

let initialState: AuthState = {
    currentUser: null,
};

export const authReducer = createReducer(
    initialState,
    on(loggedInAction, (state, { currentUser }) => ({
        ...state,
        currentUser,
    })),
    on(loggedOutAction, state => ({
        ...state,

        currentUser: null,
    })),
    on(refreshedAction, (state, { currentUser }) => ({
        ...state,
        currentUser,
    })),
);

const localStorageKey = 'auth';

export function saveStateReducer(reducer: ActionReducer<any>): ActionReducer<any> {
    return (state, action) => {
        if (action.type === '@ngrx/store/init') {
            let stored = localStorage.getItem(localStorageKey);
            if (stored) {
                let auth = JSON.parse(stored);
                if (!auth.currentUser) {
                    state = {
                        ...state,
                        auth: initialState,
                    };
                } else {
                    state = {
                        ...state,
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
        }

        let result = reducer(state, action);
        if (action.type === loggedInAction.type || action.type === refreshedAction.type) {
            localStorage.setItem(localStorageKey, JSON.stringify(result.auth));
        } else if (action.type === loggedOutAction.type) {
            localStorage.removeItem(localStorageKey);
        }

        return result;
    };
}
