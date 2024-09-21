import { RouterReducerState } from '@ngrx/router-store';
import { AuthState } from './auth.state';
import { ProfileState } from './profile.state';
import { RepositoriesState } from './repositories.state';
import { RepositoryState } from './repository.state';
import { RolesState } from './roles.state';
import { UserState } from './users.state';
import { WebSocketState } from './web-socket.state';

export interface AppState {
    router: RouterReducerState;
    auth: AuthState;
    webSocket: WebSocketState;
    roles: RolesState;
    users: UserState;
    profile: ProfileState;
    repositories: RepositoriesState;
    repository: RepositoryState;
}
