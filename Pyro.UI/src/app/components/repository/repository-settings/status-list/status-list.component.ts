import { disableStatus, enableStatus, loadStatuses } from '@actions/repository-statuses.actions';
import { AsyncPipe } from '@angular/common';
import { Component, input, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TagComponent } from '@controls/tag/tag.component';
import { DataSourceDirective } from '@directives/data-source.directive';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import { IssueStatus } from '@services/issue-status.service';
import { AppState } from '@states/app.state';
import { selectHasPermission } from '@states/auth.state';
import { DataSourceState } from '@states/data-source.state';
import { selectStatuses } from '@states/repository.state';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';

@Component({
    selector: 'status-list',
    imports: [AsyncPipe, ButtonModule, DataSourceDirective, RouterLink, TableModule, TagComponent],
    templateUrl: './status-list.component.html',
    styleUrl: './status-list.component.css',
})
export class StatusListComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public statuses$: Observable<DataSourceState<IssueStatus>> = this.store.select(selectStatuses);
    public hasManagePermission$: Observable<boolean> = this.store.select(
        selectHasPermission(PyroPermissions.IssueManage),
    );

    public constructor(private readonly store: Store<AppState>) {}

    public ngOnInit(): void {
        this.store.dispatch(loadStatuses({ repositoryName: this.repositoryName() }));
    }

    public enableStatus(status: IssueStatus): void {
        this.store.dispatch(
            enableStatus({ repositoryName: this.repositoryName(), statusId: status.id }),
        );
    }

    public disableStatus(status: IssueStatus): void {
        this.store.dispatch(
            disableStatus({ repositoryName: this.repositoryName(), statusId: status.id }),
        );
    }
}
