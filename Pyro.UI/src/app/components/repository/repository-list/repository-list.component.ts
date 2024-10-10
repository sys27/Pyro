import {
    loadRepositories,
    repositoriesNextPage,
    repositoriesPreviousPage,
} from '@actions/repositories.actions';
import { AsyncPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DataSourceDirective } from '@directives/data-source.directive';
import { Store } from '@ngrx/store';
import { RepositoryItem } from '@services/repository.service';
import { AppState } from '@states/app.state';
import { DataSourceState } from '@states/data-source.state';
import { currentPageSelector, hasNextSelector, hasPreviousSelector } from '@states/paged.state';
import { repositoriesSelector } from '@states/repositories.state';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';

@Component({
    selector: 'repo-list',
    standalone: true,
    imports: [AsyncPipe, ButtonModule, DataSourceDirective, RouterModule, TableModule],
    templateUrl: './repository-list.component.html',
    styleUrl: './repository-list.component.css',
})
export class RepositoryListComponent implements OnInit {
    public repositories$: Observable<DataSourceState<RepositoryItem>> = this.store.select(
        currentPageSelector(repositoriesSelector),
    );
    public readonly isPreviousEnabled$: Observable<boolean> = this.store.select(
        hasPreviousSelector(repositoriesSelector),
    );
    public readonly isNextEnabled$: Observable<boolean> = this.store.select(
        hasNextSelector(repositoriesSelector),
    );

    public constructor(private readonly store: Store<AppState>) {}

    public ngOnInit(): void {
        this.store.dispatch(loadRepositories());
    }

    public onPrevious(): void {
        this.store.dispatch(repositoriesPreviousPage());
    }

    public onNext(): void {
        this.store.dispatch(repositoriesNextPage());
    }
}
