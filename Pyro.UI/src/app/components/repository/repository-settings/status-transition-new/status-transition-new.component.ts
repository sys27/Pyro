import { AsyncPipe } from '@angular/common';
import { Component, Injector, input, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ValidationSummaryComponent } from '@controls/validation-summary';
import { IssueStatus, IssueStatusService } from '@services/issue-status.service';
import { createErrorHandler } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { finalize, Observable, shareReplay } from 'rxjs';

@Component({
    selector: 'status-transition-new',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        DropdownModule,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './status-transition-new.component.html',
    styleUrl: './status-transition-new.component.css',
})
export class StatusTransitionNewComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly form = this.formBuilder.group(
        {
            fromId: ['', Validators.required],
            toId: ['', Validators.required],
        },
        {
            validators: form => {
                let fromId = form.get('fromId')?.value;
                let toId = form.get('toId')?.value;

                if (fromId === toId) {
                    return { sameAsFrom: 'The from status and the to status cannot be the same' };
                }

                return null;
            },
        },
    );
    public readonly isLoading = signal<boolean>(false);

    public statuses$: Observable<IssueStatus[]> | undefined;

    public constructor(
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly statusService: IssueStatusService,
    ) {}

    public ngOnInit(): void {
        this.statuses$ = this.statusService
            .getStatuses(this.repositoryName())
            .pipe(createErrorHandler(this.injector), shareReplay(1));
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.isLoading.set(true);

        let transition = {
            fromId: this.form.value.fromId!,
            toId: this.form.value.toId!,
        };

        this.statusService
            .createStatusTransition(this.repositoryName(), transition)
            .pipe(
                createErrorHandler(this.injector),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe(() => {
                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
