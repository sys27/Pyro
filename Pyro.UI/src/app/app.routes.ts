import { Routes } from '@angular/router';
import { authGuard } from './auth.guard';
import { LoginComponent } from './components/login/login.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import {
    RepositoryCodeComponent,
    RepositoryComponent,
    RepositoryFileComponent,
    RepositoryIssuesComponent,
    RepositoryListComponent,
    RepositoryNewComponent,
    RepositoryPullRequqestsComponent,
    RepositorySettingsComponent,
} from './components/repository';
import {
    AccessTokenListComponent,
    AccessTokenNewComponent,
    ProfileComponent,
    SettingsComponent,
} from './components/settings';
import { UserEditComponent, UserListComponent, UserNewComponent } from './components/user';
import { urlMatcher } from './url.matcher';

export const routes: Routes = [
    { path: 'repositories', component: RepositoryListComponent, canActivate: [authGuard] },
    { path: 'repositories/new', component: RepositoryNewComponent, canActivate: [authGuard] },
    {
        path: 'repositories/:name',
        component: RepositoryComponent,
        canActivate: [authGuard],
        children: [
            {
                matcher: urlMatcher('code'),
                component: RepositoryCodeComponent,
                canActivate: [authGuard],
            },
            {
                matcher: urlMatcher('file'),
                component: RepositoryFileComponent,
                canActivate: [authGuard],
            },
            {
                path: 'issues',
                component: RepositoryIssuesComponent,
                canActivate: [authGuard],
            },
            {
                path: 'pull-requests',
                component: RepositoryPullRequqestsComponent,
                canActivate: [authGuard],
            },
            {
                path: 'settings',
                component: RepositorySettingsComponent,
                canActivate: [authGuard],
            },
            { path: '', redirectTo: 'code', pathMatch: 'full' },
        ],
    },

    {
        path: 'settings',
        component: SettingsComponent,
        canActivate: [authGuard],
        children: [
            { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
            {
                path: 'access-tokens/new',
                component: AccessTokenNewComponent,
                canActivate: [authGuard],
            },
            {
                path: 'access-tokens',
                component: AccessTokenListComponent,
                canActivate: [authGuard],
                children: [],
            },
            { path: '', redirectTo: 'profile', pathMatch: 'full' },
        ],
    },

    { path: 'users', component: UserListComponent, canActivate: [authGuard] },
    { path: 'users/new', component: UserNewComponent, canActivate: [authGuard] },
    { path: 'users/:login', component: UserEditComponent, canActivate: [authGuard] },

    { path: 'login', component: LoginComponent },
    { path: '', redirectTo: 'repositories', pathMatch: 'full' },
    { path: '**', component: NotFoundComponent },
];
