import { Routes } from '@angular/router';
import { authGuard } from './auth.guard';
import { LoginComponent } from './components/login/login.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { ProfileComponent } from './components/profile/profile.component';
import { RepositoryListComponent } from './components/repository-list/repository-list.component';
import { RepositoryComponent } from './components/repository/repository.component';
import { UserEditComponent } from './components/user-edit/user-edit.component';
import { UserListComponent } from './components/user-list/user-list.component';
import { UserNewComponent } from './components/user-new/user-new.component';

export const routes: Routes = [
    { path: 'repositories', component: RepositoryListComponent, canActivate: [authGuard] },
    { path: 'repositories/:name', component: RepositoryComponent, canActivate: [authGuard] },
    { path: 'profile', component: ProfileComponent, canActivate: [authGuard] },
    { path: 'users', component: UserListComponent, canActivate: [authGuard] },
    { path: 'users/new', component: UserNewComponent, canActivate: [authGuard] },
    { path: 'users/:login', component: UserEditComponent, canActivate: [authGuard] },
    { path: 'login', component: LoginComponent },
    { path: '', redirectTo: 'repositories', pathMatch: 'full' },
    { path: '**', component: NotFoundComponent },
];
