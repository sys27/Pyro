import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, input, OnDestroy, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { PyroPermissions } from '@models/pyro-permissions';
import { ResponseError } from '@models/response';
import { ColorPipe } from '@pipes/color.pipe';
import { AuthService } from '@services/auth.service';
import { Label, LabelService } from '@services/label.service';
import { mapErrorToEmpty } from '@services/operators';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { BehaviorSubject, map, Observable, shareReplay, switchMap } from 'rxjs';

@Component({
    selector: 'label-list',
    standalone: true,
    imports: [AsyncPipe, ColorPipe, ButtonModule, RouterLink, TableModule],
    templateUrl: './label-list.component.html',
    styleUrl: './label-list.component.css',
})
export class LabelListComponent implements OnInit, OnDestroy {
    public readonly repositoryName = input.required<string>();
    private readonly refreshLabels$ = new BehaviorSubject<void>(undefined);
    public labels$: Observable<Label[]> | undefined;
    public hasManagePermission$: Observable<boolean> | undefined;

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly messageService: MessageService,
        private readonly labelService: LabelService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.labels$ = this.refreshLabels$.pipe(
            switchMap(() =>
                this.labelService.getLabels(this.repositoryName()).pipe(mapErrorToEmpty),
            ),
            shareReplay(1),
        );
        this.hasManagePermission$ = this.authService.currentUser.pipe(
            takeUntilDestroyed(this.destroyRef),
            map(user => user?.hasPermission(PyroPermissions.RepositoryManage) ?? false),
        );
    }

    public ngOnDestroy(): void {
        this.refreshLabels$.complete();
    }

    public deleteLabel(label: Label): void {
        this.labelService.deleteLabel(this.repositoryName(), label.id).subscribe(response => {
            if (response instanceof ResponseError) {
                return;
            }

            this.messageService.add({
                severity: 'success',
                summary: 'Success',
                detail: `Label ${label.name} deleted`,
            });

            this.refreshLabels$.next();
        });
    }
}
