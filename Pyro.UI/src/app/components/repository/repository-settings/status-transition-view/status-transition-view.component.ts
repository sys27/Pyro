import {
    deleteStatusTransition,
    loadStatusTransitions,
} from '@actions/repository-statuses.actions';
import { AsyncPipe } from '@angular/common';
import { Component, input, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TagComponent } from '@controls/tag/tag.component';
import { DataSourceDirective } from '@directives/data-source.directive';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import { IssueStatusTransition } from '@services/issue-status.service';
import { AppState } from '@states/app.state';
import { selectHasPermission } from '@states/auth.state';
import { DataSourceState } from '@states/data-source.state';
import { selectStatusTransitions } from '@states/repository.state';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';

@Component({
    selector: 'status-transition-view',
    imports: [AsyncPipe, ButtonModule, DataSourceDirective, RouterLink, TableModule, TagComponent],
    templateUrl: './status-transition-view.component.html',
    styleUrl: './status-transition-view.component.css',
})
export class StatusTransitionViewComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public transitions$: Observable<DataSourceState<IssueStatusTransition>> =
        this.store.select(selectStatusTransitions);
    public hasManagePermission$: Observable<boolean> = this.store.select(
        selectHasPermission(PyroPermissions.RepositoryManage),
    );

    public constructor(private readonly store: Store<AppState>) {}

    public ngOnInit(): void {
        this.store.dispatch(loadStatusTransitions({ repositoryName: this.repositoryName() }));
    }

    public deleteTransition(transition: IssueStatusTransition): void {
        this.store.dispatch(
            deleteStatusTransition({
                repositoryName: this.repositoryName(),
                transitionId: transition.id,
            }),
        );
    }
}
