import { ResolveFn, Routes, UrlMatcher } from '@angular/router';
import { PyroPermissions } from '@models/pyro-permissions';
import { authGuard } from './auth.guard';
import { ForbiddenComponent } from './components/forbidden/forbidden.component';
import { LoginComponent } from './components/login/login.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import {
    LabelEditComponent,
    LabelListComponent,
    LabelNewComponent,
    RepositoryCodeComponent,
    RepositoryComponent,
    RepositoryFileComponent,
    RepositoryIssueComponent,
    RepositoryIssueEditComponent,
    RepositoryIssueNewComponent,
    RepositoryIssuesComponent,
    RepositoryListComponent,
    RepositoryNewComponent,
    RepositoryPullRequqestsComponent,
    RepositorySettingsComponent,
    StatusEditComponent,
    StatusListComponent,
    StatusNewComponent,
    StatusTransitionNewComponent,
    StatusTransitionViewComponent,
} from './components/repository';
import {
    AccessTokenListComponent,
    AccessTokenNewComponent,
    ProfileEditComponent,
    SettingsComponent,
} from './components/settings';
import { UserEditComponent, UserListComponent, UserNewComponent } from './components/user';

export const routes: Routes = [
    {
        path: 'repositories',
        component: RepositoryListComponent,
        canActivate: [authGuard(PyroPermissions.RepositoryView)],
    },
    {
        path: 'repositories/new',
        component: RepositoryNewComponent,
        canActivate: [authGuard(PyroPermissions.RepositoryEdit)],
    },
    {
        path: 'repositories/:repositoryName',
        component: RepositoryComponent,
        canActivate: [authGuard(PyroPermissions.RepositoryView)],
        children: [
            {
                matcher: prefixMatcher('code'),
                resolve: {
                    branchOrPath: branchOrPathResolver('code'),
                },
                component: RepositoryCodeComponent,
                canActivate: [authGuard(PyroPermissions.RepositoryView)],
            },
            {
                matcher: prefixMatcher('file'),
                resolve: {
                    branchOrPath: branchOrPathResolver('file'),
                },
                component: RepositoryFileComponent,
                canActivate: [authGuard(PyroPermissions.RepositoryView)],
            },
            {
                path: 'issues/new',
                component: RepositoryIssueNewComponent,
                canActivate: [authGuard(PyroPermissions.IssueEdit)],
            },
            {
                path: 'issues/:issueNumber',
                component: RepositoryIssueComponent,
                canActivate: [authGuard(PyroPermissions.IssueView)],
            },
            {
                path: 'issues/:issueNumber/edit',
                component: RepositoryIssueEditComponent,
                canActivate: [authGuard(PyroPermissions.IssueEdit)],
            },
            {
                path: 'issues',
                component: RepositoryIssuesComponent,
                canActivate: [authGuard(PyroPermissions.IssueView)],
            },
            {
                path: 'pull-requests',
                component: RepositoryPullRequqestsComponent,
                canActivate: [authGuard()],
            },
            {
                path: 'settings',
                component: RepositorySettingsComponent,
                canActivate: [authGuard(PyroPermissions.RepositoryView)],
                children: [
                    {
                        path: 'labels/new',
                        component: LabelNewComponent,
                        canActivate: [authGuard(PyroPermissions.RepositoryManage)],
                    },
                    {
                        path: 'labels/:labelId',
                        component: LabelEditComponent,
                        canActivate: [authGuard(PyroPermissions.RepositoryManage)],
                    },
                    {
                        path: 'labels',
                        component: LabelListComponent,
                        canActivate: [authGuard(PyroPermissions.RepositoryView)],
                    },

                    {
                        path: 'statuses/transitions/new',
                        component: StatusTransitionNewComponent,
                        canActivate: [authGuard(PyroPermissions.RepositoryManage)],
                    },
                    {
                        path: 'statuses/transitions',
                        component: StatusTransitionViewComponent,
                        canActivate: [authGuard(PyroPermissions.RepositoryView)],
                    },

                    {
                        path: 'statuses/new',
                        component: StatusNewComponent,
                        canActivate: [authGuard(PyroPermissions.RepositoryManage)],
                    },
                    {
                        path: 'statuses/:statusId',
                        component: StatusEditComponent,
                        canActivate: [authGuard(PyroPermissions.RepositoryManage)],
                    },
                    {
                        path: 'statuses',
                        component: StatusListComponent,
                        canActivate: [authGuard(PyroPermissions.RepositoryView)],
                    },

                    { path: '', redirectTo: 'labels', pathMatch: 'full' },
                ],
            },
            { path: '', redirectTo: 'code', pathMatch: 'full' },
        ],
    },

    {
        path: 'settings',
        component: SettingsComponent,
        canActivate: [authGuard()],
        children: [
            { path: 'profile', component: ProfileEditComponent, canActivate: [authGuard()] },
            {
                path: 'access-tokens/new',
                component: AccessTokenNewComponent,
                canActivate: [authGuard()],
            },
            {
                path: 'access-tokens',
                component: AccessTokenListComponent,
                canActivate: [authGuard()],
                children: [],
            },
            { path: '', redirectTo: 'profile', pathMatch: 'full' },
        ],
    },

    {
        path: 'users',
        component: UserListComponent,
        canActivate: [authGuard(PyroPermissions.UserView)],
    },
    {
        path: 'users/new',
        component: UserNewComponent,
        canActivate: [authGuard(PyroPermissions.UserEdit)],
    },
    {
        path: 'users/:login',
        component: UserEditComponent,
        canActivate: [authGuard(PyroPermissions.UserEdit)],
    },

    { path: 'login', component: LoginComponent },
    { path: '', redirectTo: 'repositories', pathMatch: 'full' },
    { path: 'forbidden', component: ForbiddenComponent },
    { path: '**', component: NotFoundComponent },
];

function prefixMatcher(prefix: string): UrlMatcher {
    return segments => {
        if (segments.length > 0 && segments[0].path === prefix) {
            return {
                consumed: segments,
            };
        }

        return null;
    };
}

function branchOrPathResolver(prefix: string): ResolveFn<string[]> {
    return route =>
        route.url.filter(segment => segment.path != prefix).map(segment => segment.path);
}
