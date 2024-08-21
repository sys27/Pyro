import { AsyncPipe } from '@angular/common';
import { Component, computed, input, OnInit, signal } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { IssueService, User } from '@services/issue.service';
import { Label, LabelService } from '@services/label.service';
import { mapErrorToEmpty, mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { finalize, Observable, shareReplay, take } from 'rxjs';
import { WithValidationComponent } from '../../../controls/with-validation/with-validation.component';

@Component({
    selector: 'repo-issue-new',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        DropdownModule,
        InputTextModule,
        MultiSelectModule,
        ReactiveFormsModule,
        WithValidationComponent,
    ],
    templateUrl: './repository-issue-new.component.html',
    styleUrl: './repository-issue-new.component.css',
})
export class RepositoryIssueNewComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly issueNumber = input<number>();
    public readonly isEditMode = computed<boolean>(() => !!this.issueNumber());
    public users$: Observable<User[]> | undefined;
    public labels$: Observable<Label[]> | undefined;
    public readonly form = this.formBuilder.group({
        title: ['', [Validators.required, Validators.maxLength(200)]],
        assigneeId: new FormControl<string | null>(null),
        labelIds: new FormControl<string[]>([]),
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly issueService: IssueService,
        private readonly labelService: LabelService,
    ) {}

    public ngOnInit(): void {
        this.users$ = this.issueService.getUsers().pipe(mapErrorToEmpty, shareReplay(1));
        this.labels$ = this.labelService
            .getLabels(this.repositoryName())
            .pipe(mapErrorToEmpty, shareReplay(1));

        if (this.isEditMode()) {
            this.issueService
                .getIssue(this.repositoryName(), this.issueNumber()!)
                .pipe(mapErrorToNull, take(1))
                .subscribe(issue => {
                    if (issue) {
                        this.form.setValue({
                            title: issue.title,
                            assigneeId: issue.assignee?.id || null,
                            labelIds: issue.labels.map(label => label.id),
                        });
                    }
                });
        }
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let issue = {
            title: this.form.value.title!,
            assigneeId: this.form.value.assigneeId!,
            labels: this.form.value.labelIds || [],
        };

        this.isLoading.set(true);

        if (this.isEditMode()) {
            this.issueService
                .updateIssue(this.repositoryName(), this.issueNumber()!, issue)
                .pipe(finalize(() => this.isLoading.set(false)))
                .subscribe(() => {
                    this.router.navigate(['../'], { relativeTo: this.route });
                });
        } else {
            this.issueService
                .createIssue(this.repositoryName(), issue)
                .pipe(finalize(() => this.isLoading.set(false)))
                .subscribe(() => {
                    this.router.navigate(['../'], { relativeTo: this.route });
                });
        }
    }
}
