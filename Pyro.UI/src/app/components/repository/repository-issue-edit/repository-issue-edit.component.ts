import { Component, Injector, input, signal } from '@angular/core';
import {
    AbstractControl,
    FormBuilder,
    FormControl,
    ReactiveFormsModule,
    ValidationErrors,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { ObservableOptionsDirective } from '@directives/observable-options.directive';
import {
    IssueStatus,
    IssueStatusService,
    IssueStatusTransition,
} from '@services/issue-status.service';
import { Issue, IssueService, User } from '@services/issue.service';
import { Label, LabelService } from '@services/label.service';
import { createErrorHandler } from '@services/operators';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { finalize, map, Observable, of, shareReplay, take } from 'rxjs';

@Component({
    selector: 'repository-issue-edit',
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
    templateUrl: './repository-issue-edit.component.html',
    styleUrl: './repository-issue-edit.component.css',
})
export class RepositoryIssueEditComponent {
    public readonly repositoryName = input.required<string>();
    public readonly issueNumber = input<number>();
    public users$: Observable<User[]> | undefined;
    public labels$: Observable<Label[]> | undefined;
    public statuses$: Observable<IssueStatus[]> | undefined;
    private issue: Issue | undefined;
    private statusTranstions$: Observable<IssueStatusTransition[]> | undefined;
    public readonly form = this.formBuilder.group({
        title: ['', [Validators.required('Title'), Validators.maxLength('Title', 200)]],
        assigneeId: new FormControl<string | null>(null),
        labelIds: new FormControl<string[]>([]),
        statusId: [
            '',
            Validators.required('Status'),
            (control: AbstractControl): Observable<ValidationErrors | null> => {
                if (!this.statusTranstions$ || !this.issue) {
                    return of(null);
                }

                return this.statusTranstions$.pipe(
                    map(transitions => {
                        let initialValue = this.issue?.status.id;
                        let currentValue = control.value;
                        if (!initialValue || initialValue == currentValue) {
                            return null;
                        }

                        let transition = transitions.find(
                            t => t.from.id === initialValue && t.to.id === currentValue,
                        );

                        return transition ? null : { invalidStatus: `Invalid status` };
                    }),
                );
            },
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
        this.labels$ = this.labelService
            .getLabels(this.repositoryName())
            .pipe(createErrorHandler(this.injector), shareReplay(1));
        this.statuses$ = this.statusService
            .getStatuses(this.repositoryName())
            .pipe(createErrorHandler(this.injector), shareReplay(1));
        this.statusTranstions$ = this.statusService
            .getStatusTransitions(this.repositoryName())
            .pipe(createErrorHandler(this.injector), shareReplay(1));
        this.issueService
            .getIssue(this.repositoryName(), this.issueNumber()!)
            .pipe(take(1), createErrorHandler(this.injector))
            .subscribe(issue => {
                if (issue) {
                    this.issue = issue;
                    this.form.setValue({
                        title: issue.title,
                        assigneeId: issue.assignee?.id || null,
                        labelIds: issue.labels.map(label => label.id),
                        statusId: issue.status.id,
                    });
                }
            });
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
            .updateIssue(this.repositoryName(), this.issueNumber()!, issue)
            .pipe(
                createErrorHandler(this.injector),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe(() => {
                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
