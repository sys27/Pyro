import { createStatusTransition, loadStatuses } from '@actions/repository-statuses.actions';
import { Component, Injector, input, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ValidationSummaryComponent } from '@controls/validation-summary';
import { DataSourceDirective } from '@directives/data-source.directive';
import { Store } from '@ngrx/store';
import { IssueStatus, IssueStatusService } from '@services/issue-status.service';
import { AppState } from '@states/app.state';
import { DataSourceState } from '@states/data-source.state';
import { selectStatuses } from '@states/repository.state';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { Observable } from 'rxjs';

@Component({
    selector: 'status-transition-new',
    imports: [
        ButtonModule,
        DataSourceDirective,
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

    public statuses$: Observable<DataSourceState<IssueStatus>> = this.store.select(selectStatuses);

    public constructor(
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly store: Store<AppState>,
        private readonly statusService: IssueStatusService,
    ) {}

    public ngOnInit(): void {
        this.store.dispatch(loadStatuses({ repositoryName: this.repositoryName() }));
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let transition = {
            fromId: this.form.value.fromId!,
            toId: this.form.value.toId!,
        };

        this.isLoading.set(true);
        this.store.dispatch(
            createStatusTransition({ repositoryName: this.repositoryName(), transition }),
        );
    }
}
