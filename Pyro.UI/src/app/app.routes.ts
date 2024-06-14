import { Routes } from '@angular/router';
import { authGuard } from './auth.guard';
import { LoginComponent } from './components/login/login.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { RepositoryListComponent } from './components/repository-list/repository-list.component';

export const routes: Routes = [
    { path: 'repositories', component: RepositoryListComponent, canActivate: [authGuard] },
    { path: 'login', component: LoginComponent },
    { path: '', redirectTo: 'repositories', pathMatch: 'full' },
    { path: '**', component: NotFoundComponent },
];
