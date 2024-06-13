import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { RepositoryListComponent } from './components/repository-list/repository-list.component';

export const routes: Routes = [
    { path: 'repositories', component: RepositoryListComponent },
    { path: 'login', component: LoginComponent },
    { path: '', redirectTo: 'repositories', pathMatch: 'full' },
    { path: '**', component: NotFoundComponent },
];
