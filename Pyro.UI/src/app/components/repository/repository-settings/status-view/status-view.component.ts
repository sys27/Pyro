import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, input, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { PyroPermissions } from '@models/pyro-permissions';
import { ResponseError } from '@models/response';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { AuthService } from '@services/auth.service';
import { IssueStatus, IssueStatusService } from '@services/issue-status.service';
import { mapErrorToEmpty } from '@services/operators';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
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
        TagModule,
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
        private readonly destroyRef: DestroyRef,
        private readonly messageService: MessageService,
        private readonly statusService: IssueStatusService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.statuses$ = this.refreshStatuses$.pipe(
            switchMap(() => this.statusService.getStatuses(this.repositoryName())),
            mapErrorToEmpty,
        );
        this.hasManagePermission$ = this.authService.currentUser.pipe(
            takeUntilDestroyed(this.destroyRef),
            map(user => user?.hasPermission(PyroPermissions.RepositoryManage) ?? false),
        );
    }

    public deleteStatus(status: IssueStatus): void {
        this.statusService.deleteStatus(this.repositoryName(), status.id).subscribe(response => {
            if (response instanceof ResponseError) {
                return;
            }

            this.messageService.add({
                severity: 'success',
                summary: 'Success',
                detail: `Status '${status.name}' deleted`,
            });

            this.refreshStatuses$.next();
        });
    }
}
