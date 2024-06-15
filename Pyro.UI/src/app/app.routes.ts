import { Routes } from '@angular/router';
import { authGuard } from './auth.guard';
import { LoginComponent } from './components/login/login.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { RepositoryListComponent } from './components/repository-list/repository-list.component';
import { RepositoryComponent } from './components/repository/repository.component';
import { UserListComponent } from './components/user-list/user-list.component';
import { UserComponent } from './components/user/user.component';

export const routes: Routes = [
    { path: 'repositories', component: RepositoryListComponent, canActivate: [authGuard] },
    { path: 'repositories/:name', component: RepositoryComponent, canActivate: [authGuard] },
    { path: 'users', component: UserListComponent, canActivate: [authGuard] },
    { path: 'users/:email', component: UserComponent, canActivate: [authGuard] },
    { path: 'login', component: LoginComponent },
    { path: '', redirectTo: 'repositories', pathMatch: 'full' },
    { path: '**', component: NotFoundComponent },
];
