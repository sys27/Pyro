import { AsyncPipe } from '@angular/common';
import { Component, input, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ResponseError } from '@models/response';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { IssueStatusService, IssueStatusTransition } from '@services/issue-status.service';
import { mapErrorToEmpty } from '@services/operators';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { BehaviorSubject, Observable, switchMap } from 'rxjs';

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
    private readonly refreshTransitions$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly messageService: MessageService,
        private readonly statusService: IssueStatusService,
    ) {}

    public ngOnInit(): void {
        this.transitions$ = this.refreshTransitions$.pipe(
            switchMap(() => this.statusService.getStatusTransitions(this.repositoryName())),
            mapErrorToEmpty,
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
