import { AsyncPipe } from '@angular/common';
import { Component, Injector, input, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TagComponent } from '@controls/tag/tag.component';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { IssueStatus, IssueStatusService } from '@services/issue-status.service';
import { createErrorHandler } from '@services/operators';
import { AppState } from '@states/app.state';
import { hasPermissionSelector } from '@states/auth.state';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { BehaviorSubject, Observable, switchMap } from 'rxjs';

@Component({
    selector: 'status-list',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        ColorPipe,
        LuminanceColorPipe,
        RouterLink,
        TableModule,
        TagComponent,
    ],
    templateUrl: './status-list.component.html',
    styleUrl: './status-list.component.css',
})
export class StatusListComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public statuses$: Observable<IssueStatus[]> | undefined;
    public hasManagePermission$: Observable<boolean> = this.store.select(
        hasPermissionSelector(PyroPermissions.IssueManage),
    );
    private readonly refreshStatuses$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly injector: Injector,
        private readonly store: Store<AppState>,
        private readonly messageService: MessageService,
        private readonly statusService: IssueStatusService,
    ) {}

    public ngOnInit(): void {
        this.statuses$ = this.refreshStatuses$.pipe(
            switchMap(() => this.statusService.getStatuses(this.repositoryName())),
            createErrorHandler(this.injector),
        );
    }

    public enableStatus(status: IssueStatus): void {
        this.statusService
            .enableStatus(this.repositoryName(), status.id)
            .pipe(createErrorHandler(this.injector))
            .subscribe(() => {
                this.messageService.add({
                    severity: 'success',
                    summary: 'Success',
                    detail: `Status '${status.name}' enabled`,
                });

                this.refreshStatuses$.next();
            });
    }

    public disableStatus(status: IssueStatus): void {
        this.statusService
            .disableStatus(this.repositoryName(), status.id)
            .pipe(createErrorHandler(this.injector))
            .subscribe(() => {
                this.messageService.add({
                    severity: 'success',
                    summary: 'Success',
                    detail: `Status '${status.name}' disabled`,
                });

                this.refreshStatuses$.next();
            });
    }
}
