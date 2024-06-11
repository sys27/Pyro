import { Routes } from '@angular/router';
import { NotFoundComponent } from './components/not-found/not-found.component';
import { RepositoryListComponent } from './components/repository-list/repository-list.component';

export const routes: Routes = [
  { path: 'repositories', component: RepositoryListComponent },
  { path: '', redirectTo: 'repositories', pathMatch: 'full' },
  { path: '**', component: NotFoundComponent }
];
