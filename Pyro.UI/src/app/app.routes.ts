import { ResolveFn, Routes, UrlMatcher } from '@angular/router';
import { authGuard } from './auth.guard';
import { LoginComponent } from './components/login/login.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import {
    RepositoryCodeComponent,
    RepositoryComponent,
    RepositoryFileComponent,
    RepositoryIssueComponent,
    RepositoryIssueNewComponent,
    RepositoryIssuesComponent,
    RepositoryListComponent,
    RepositoryNewComponent,
    RepositoryPullRequqestsComponent,
    RepositorySettingsComponent,
    TagEditComponent,
    TagListComponent,
    TagNewComponent,
} from './components/repository';
import {
    AccessTokenListComponent,
    AccessTokenNewComponent,
    ProfileComponent,
    SettingsComponent,
} from './components/settings';
import { UserEditComponent, UserListComponent, UserNewComponent } from './components/user';

export const routes: Routes = [
    { path: 'repositories', component: RepositoryListComponent, canActivate: [authGuard] },
    { path: 'repositories/new', component: RepositoryNewComponent, canActivate: [authGuard] },
    {
        path: 'repositories/:repositoryName',
        component: RepositoryComponent,
        canActivate: [authGuard],
        children: [
            {
                matcher: prefixMatcher('code'),
                resolve: {
                    branchOrPath: branchOrPathResolver('code'),
                },
                component: RepositoryCodeComponent,
                canActivate: [authGuard],
            },
            {
                matcher: prefixMatcher('file'),
                resolve: {
                    branchOrPath: branchOrPathResolver('file'),
                },
                component: RepositoryFileComponent,
                canActivate: [authGuard],
            },
            {
                path: 'issues/new',
                component: RepositoryIssueNewComponent,
                canActivate: [authGuard],
            },
            {
                path: 'issues/:issueNumber',
                component: RepositoryIssueComponent,
                canActivate: [authGuard],
            },
            {
                path: 'issues/:issueNumber/edit',
                component: RepositoryIssueNewComponent,
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
                children: [
                    { path: 'tags/new', component: TagNewComponent, canActivate: [authGuard] },
                    { path: 'tags/:tagId', component: TagEditComponent, canActivate: [authGuard] },
                    { path: 'tags', component: TagListComponent, canActivate: [authGuard] },
                    { path: '', redirectTo: 'tags', pathMatch: 'full' },
                ],
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
