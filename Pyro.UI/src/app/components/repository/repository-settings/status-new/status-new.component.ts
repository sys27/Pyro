import { createStatus } from '@actions/repository-statuses.actions';
import { Component, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Color } from '@models/color';
import { Store } from '@ngrx/store';
import { IssueStatusService } from '@services/issue-status.service';
import { AppState } from '@states/app.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';
import { filter } from 'rxjs';

@Component({
    selector: 'status-new',
    standalone: true,
    imports: [
        AutoFocusModule,
        ButtonModule,
        ColorPickerModule,
        InputTextModule,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './status-new.component.html',
    styleUrl: './status-new.component.css',
})
export class StatusNewComponent {
    public readonly repositoryName = input.required<string>();
    public readonly form = this.formBuilder.nonNullable.group({
        name: [
            '',
            [Validators.required('Name'), Validators.maxLength('Name', 50)],
            [
                Validators.exists(
                    value => `The '${value}' status already exists`,
                    value =>
                        this.statusService
                            .getStatuses(this.repositoryName(), value)
                            .pipe(filter(status => status.some(x => x.name == value))),
                ),
            ],
        ],
        color: [{} as Color, [Validators.required('Color')]],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly store: Store<AppState>,
        private readonly statusService: IssueStatusService,
    ) {}

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let status = {
            name: this.form.value.name!,
            color: this.form.value.color!,
        };

        this.isLoading.set(true);
        this.store.dispatch(createStatus({ repositoryName: this.repositoryName(), status }));
    }
}
