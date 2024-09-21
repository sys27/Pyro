import { editIssue, editIssueComponentOpened } from '@actions/issues.actions';
import { Component, DestroyRef, input, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
    AbstractControl,
    FormBuilder,
    FormControl,
    ReactiveFormsModule,
    ValidationErrors,
} from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { DataSourceDirective } from '@directives/data-source.directive';
import { Store } from '@ngrx/store';
import { IssueStatus, IssueStatusTransition } from '@services/issue-status.service';
import { Issue, User } from '@services/issue.service';
import { Label } from '@services/label.service';
import { AppState } from '@states/app.state';
import { DataSourceState } from '@states/data-source.state';
import {
    selectEnabledLabels,
    selectEnabledStatuses,
    selectSelectedIssue,
    selectStatusTransitions,
    selectUsers,
} from '@states/repository.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { combineLatest, filter, map, Observable } from 'rxjs';

@Component({
    selector: 'repository-issue-edit',
    standalone: true,
    imports: [
        AutoFocusModule,
        ButtonModule,
        DataSourceDirective,
        DropdownModule,
        InputTextModule,
        MultiSelectModule,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './repository-issue-edit.component.html',
    styleUrl: './repository-issue-edit.component.css',
})
export class RepositoryIssueEditComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly issueNumber = input.required<number>();
    public users$: Observable<DataSourceState<User>> = this.store.select(selectUsers);
    public labels$: Observable<DataSourceState<Label>> = this.store.select(selectEnabledLabels);
    public statuses$: Observable<DataSourceState<IssueStatus>> =
        this.store.select(selectEnabledStatuses);
    private statusTranstions$: Observable<DataSourceState<IssueStatusTransition>> =
        this.store.select(selectStatusTransitions);
    private issue$: Observable<Issue | null> = this.store.select(selectSelectedIssue);
    public readonly form = this.formBuilder.group({
        title: ['', [Validators.required('Title'), Validators.maxLength('Title', 200)]],
        assigneeId: new FormControl<string | null>(null),
        labelIds: new FormControl<string[]>([]),
        statusId: [
            '',
            Validators.required('Status'),
            (control: AbstractControl): Observable<ValidationErrors | null> => {
                return combineLatest([this.statusTranstions$, this.issue$]).pipe(
                    map(([transitions, issue]) => {
                        let initialValue = issue?.status.id;
                        let currentValue = control.value;
                        if (!initialValue || initialValue == currentValue) {
                            return null;
                        }

                        let transition = transitions.data.find(
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
        private readonly destroyRef: DestroyRef,
        private readonly formBuilder: FormBuilder,
        private readonly store: Store<AppState>,
    ) {}

    public ngOnInit(): void {
        this.store.dispatch(
            editIssueComponentOpened({
                repositoryName: this.repositoryName(),
                issueNumber: this.issueNumber(),
            }),
        );

        this.issue$
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                filter(issue => !!issue),
            )
            .subscribe(issue => {
                this.form.setValue({
                    title: issue.title,
                    assigneeId: issue.assignee?.id || null,
                    labelIds: issue.labels.map(label => label.id),
                    statusId: issue.status.id,
                });
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
        this.store.dispatch(
            editIssue({
                repositoryName: this.repositoryName(),
                issueNumber: this.issueNumber(),
                issue,
            }),
        );
    }
}
