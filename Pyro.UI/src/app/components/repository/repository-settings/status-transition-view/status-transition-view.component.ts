import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, input, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { PyroPermissions } from '@models/pyro-permissions';
import { ResponseError } from '@models/response';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { AuthService } from '@services/auth.service';
import { IssueStatusService, IssueStatusTransition } from '@services/issue-status.service';
import { mapErrorToEmpty } from '@services/operators';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { BehaviorSubject, map, Observable, switchMap } from 'rxjs';

@Component({
    selector: 'status-transition-view',
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
    templateUrl: './status-transition-view.component.html',
    styleUrl: './status-transition-view.component.css',
})
export class StatusTransitionViewComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public transitions$: Observable<IssueStatusTransition[]> | undefined;
    public hasManagePermission$: Observable<boolean> | undefined;
    private readonly refreshTransitions$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly messageService: MessageService,
        private readonly statusService: IssueStatusService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.transitions$ = this.refreshTransitions$.pipe(
            switchMap(() => this.statusService.getStatusTransitions(this.repositoryName())),
            mapErrorToEmpty,
        );
        this.hasManagePermission$ = this.authService.currentUser.pipe(
            takeUntilDestroyed(this.destroyRef),
            map(user => user?.hasPermission(PyroPermissions.RepositoryManage) ?? false),
        );
    }

    public deleteTransition(transition: IssueStatusTransition): void {
        this.statusService
            .deleteStatusTransition(this.repositoryName(), transition.id)
            .subscribe(response => {
                if (response instanceof ResponseError) {
                    return;
                }

                this.messageService.add({
                    severity: 'success',
                    summary: 'Success',
                    detail: `Status Transition deleted`,
                });

                this.refreshTransitions$.next();
            });
    }
}
