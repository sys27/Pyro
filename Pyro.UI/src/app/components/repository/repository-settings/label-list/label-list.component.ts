import { disableLabel, enableLabel, loadLabels } from '@actions/repository-labels.actions';
import { AsyncPipe } from '@angular/common';
import { Component, input, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DataSourceDirective } from '@directives/data-source.directive';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import { ColorPipe } from '@pipes/color.pipe';
import { Label } from '@services/label.service';
import { AppState } from '@states/app.state';
import { selectHasPermission } from '@states/auth.state';
import { DataSourceState } from '@states/data-source.state';
import { selectLabels } from '@states/repository.state';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';

@Component({
    selector: 'label-list',
    imports: [AsyncPipe, ButtonModule, ColorPipe, DataSourceDirective, RouterLink, TableModule],
    templateUrl: './label-list.component.html',
    styleUrl: './label-list.component.css',
})
export class LabelListComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public labels$: Observable<DataSourceState<Label>> = this.store.select(selectLabels);
    public hasManagePermission$: Observable<boolean> = this.store.select(
        selectHasPermission(PyroPermissions.RepositoryManage),
    );

    public constructor(private readonly store: Store<AppState>) {}

    public ngOnInit(): void {
        this.store.dispatch(loadLabels({ repositoryName: this.repositoryName() }));
    }

    public enableLabel(label: Label): void {
        this.store.dispatch(
            enableLabel({ repositoryName: this.repositoryName(), labelId: label.id }),
        );
    }

    public disableLabel(label: Label): void {
        this.store.dispatch(
            disableLabel({ repositoryName: this.repositoryName(), labelId: label.id }),
        );
    }
}
