import { AsyncPipe } from '@angular/common';
import { Component, input, OnInit, signal } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { IssueStatus, IssueStatusService } from '@services/issue-status.service';
import { IssueService, User } from '@services/issue.service';
import { Label, LabelService } from '@services/label.service';
import { mapErrorToEmpty, mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { finalize, Observable, shareReplay } from 'rxjs';

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
        ValidationSummaryComponent,
    ],
    templateUrl: './repository-issue-new.component.html',
    styleUrl: './repository-issue-new.component.css',
})
export class RepositoryIssueNewComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public users$: Observable<User[]> | undefined;
    public labels$: Observable<Label[]> | undefined;
    public statuses$: Observable<IssueStatus[]> | undefined;
    public readonly form = this.formBuilder.group({
        title: ['', [Validators.required('Title'), Validators.maxLength('Title', 200)]],
        assigneeId: new FormControl<string | null>(null),
        labelIds: new FormControl<string[]>([]),
        statusId: ['', Validators.required('Status')],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly issueService: IssueService,
        private readonly labelService: LabelService,
        private readonly statusService: IssueStatusService,
    ) {}

    public ngOnInit(): void {
        this.users$ = this.issueService.getUsers().pipe(mapErrorToEmpty, shareReplay(1));
        this.labels$ = this.labelService
            .getLabels(this.repositoryName())
            .pipe(mapErrorToEmpty, shareReplay(1));
        this.statuses$ = this.statusService
            .getStatuses(this.repositoryName())
            .pipe(mapErrorToEmpty, shareReplay(1));
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let issue = {
            title: this.form.value.title!,
            assigneeId: this.form.value.assigneeId!,
            labels: this.form.value.labelIds || [],
            statusId: this.form.value.statusId!,
        };

        this.isLoading.set(true);

        this.issueService
            .createIssue(this.repositoryName(), issue)
            .pipe(
                mapErrorToNull,
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe(response => {
                if (response === null) {
                    return;
                }

                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
