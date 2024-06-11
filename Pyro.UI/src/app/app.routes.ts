import { Routes } from '@angular/router';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { RepositoryListComponent } from './components/repository-list/repository-list.component';
import { LoginComponent } from './components/login/login.component';

export const routes: Routes = [
    { path: 'repositories', component: RepositoryListComponent },
    { path: 'login', component: LoginComponent },
    { path: '', redirectTo: 'repositories', pathMatch: 'full' },
    { path: '**', component: NotFoundComponent }
];
