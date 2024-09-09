import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, Injector, input, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { TagComponent } from '@controls/tag/tag.component';
import { PyroPermissions } from '@models/pyro-permissions';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { AuthService } from '@services/auth.service';
import { IssueStatus, IssueStatusService } from '@services/issue-status.service';
import { createErrorHandler } from '@services/operators';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { BehaviorSubject, map, Observable, switchMap } from 'rxjs';

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
    templateUrl: './status-view.component.html',
    styleUrl: './status-view.component.css',
})
export class StatusListComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public statuses$: Observable<IssueStatus[]> | undefined;
    public hasManagePermission$: Observable<boolean> | undefined;
    private readonly refreshStatuses$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly injector: Injector,
        private readonly destroyRef: DestroyRef,
        private readonly messageService: MessageService,
        private readonly statusService: IssueStatusService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.statuses$ = this.refreshStatuses$.pipe(
            switchMap(() => this.statusService.getStatuses(this.repositoryName())),
            createErrorHandler(this.injector),
        );
        this.hasManagePermission$ = this.authService.currentUser.pipe(
            takeUntilDestroyed(this.destroyRef),
            map(user => user?.hasPermission(PyroPermissions.IssueManage) ?? false),
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
