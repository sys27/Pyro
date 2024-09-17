import { Component, Injector, input, OnInit, signal } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { ObservableOptionsDirective } from '@directives/observable-options.directive';
import { IssueStatus, IssueStatusService } from '@services/issue-status.service';
import { IssueService, User } from '@services/issue.service';
import { Label, LabelService } from '@services/label.service';
import { createErrorHandler } from '@services/operators';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { finalize, map, Observable, shareReplay } from 'rxjs';

@Component({
    selector: 'repo-issue-new',
    standalone: true,
    imports: [
        AutoFocusModule,
        ButtonModule,
        DropdownModule,
        InputTextModule,
        MultiSelectModule,
        ObservableOptionsDirective,
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
        initialComment: [
            '',
            [Validators.required('Initial comment'), Validators.maxLength('Initial comment', 2000)],
        ],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly issueService: IssueService,
        private readonly labelService: LabelService,
        private readonly statusService: IssueStatusService,
    ) {}

    public ngOnInit(): void {
        this.users$ = this.issueService
            .getUsers()
            .pipe(createErrorHandler(this.injector), shareReplay(1));
        this.labels$ = this.labelService.getLabels(this.repositoryName()).pipe(
            createErrorHandler(this.injector),
            shareReplay(1),
            map(labels => labels.filter(label => !label.isDisabled)),
        );
        this.statuses$ = this.statusService.getStatuses(this.repositoryName()).pipe(
            createErrorHandler(this.injector),
            shareReplay(1),
            map(statuses => statuses.filter(status => !status.isDisabled)),
        );
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
            initialComment: this.form.value.initialComment!,
        };

        this.isLoading.set(true);

        this.issueService
            .createIssue(this.repositoryName(), issue)
            .pipe(
                createErrorHandler(this.injector),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe(() => {
                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
