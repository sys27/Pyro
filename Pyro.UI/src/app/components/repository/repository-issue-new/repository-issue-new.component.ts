import { createIssue, newIssueComponentOpened } from '@actions/issues.actions';
import { Component, input, OnInit, signal } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { DataSourceDirective } from '@directives/data-source.directive';
import { Store } from '@ngrx/store';
import { IssueStatus } from '@services/issue-status.service';
import { User } from '@services/issue.service';
import { Label } from '@services/label.service';
import { AppState } from '@states/app.state';
import { DataSourceState } from '@states/data-source.state';
import { selectEnabledLabels, selectEnabledStatuses, selectUsers } from '@states/repository.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { Observable } from 'rxjs';

@Component({
    selector: 'repo-issue-new',
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
    templateUrl: './repository-issue-new.component.html',
    styleUrl: './repository-issue-new.component.css',
})
export class RepositoryIssueNewComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public users$: Observable<DataSourceState<User>> = this.store.select(selectUsers);
    public labels$: Observable<DataSourceState<Label>> = this.store.select(selectEnabledLabels);
    public statuses$: Observable<DataSourceState<IssueStatus>> =
        this.store.select(selectEnabledStatuses);
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
        private readonly formBuilder: FormBuilder,
        private readonly store: Store<AppState>,
    ) {}

    public ngOnInit(): void {
        this.store.dispatch(newIssueComponentOpened({ repositoryName: this.repositoryName() }));
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
        this.store.dispatch(createIssue({ repositoryName: this.repositoryName(), issue }));
    }
}
