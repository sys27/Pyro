import { AsyncPipe } from '@angular/common';
import { Component, input, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { WithValidationComponent } from '@controls/with-validation/with-validation.component';
import { IssueStatus, IssueStatusService } from '@services/issue-status.service';
import { mapErrorToEmpty, mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { Observable, shareReplay } from 'rxjs';

@Component({
    selector: 'status-transition-new',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        DropdownModule,
        ReactiveFormsModule,
        WithValidationComponent,
    ],
    templateUrl: './status-transition-new.component.html',
    styleUrl: './status-transition-new.component.css',
})
export class StatusTransitionNewComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    // TODO: add validator
    public readonly form = this.formBuilder.group({
        fromId: ['', Validators.required],
        toId: ['', Validators.required],
    });
    public readonly isLoading = signal<boolean>(false);

    public statuses$: Observable<IssueStatus[]> | undefined;

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly statusService: IssueStatusService,
    ) {}

    public ngOnInit(): void {
        this.statuses$ = this.statusService
            .getStatuses(this.repositoryName())
            .pipe(mapErrorToEmpty, shareReplay(1));
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
            .pipe(mapErrorToNull)
            .subscribe(response => {
                if (response === null) {
                    this.isLoading.set(false);
                    return;
                }

                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
