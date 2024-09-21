import { loadStatuses, updateStatus } from '@actions/repository-statuses.actions';
import { Component, DestroyRef, input, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Color } from '@models/color';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { selectStatuses } from '@states/repository.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';
import { filter, map } from 'rxjs';

@Component({
    selector: 'status-edit',
    standalone: true,
    imports: [
        AutoFocusModule,
        ButtonModule,
        ColorPickerModule,
        InputTextModule,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './status-edit.component.html',
    styleUrl: './status-edit.component.css',
})
export class StatusEditComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly statusId = input.required<string>();
    public readonly form = this.formBuilder.nonNullable.group({
        name: ['', [Validators.required('Name'), Validators.maxLength('Name', 50)]],
        color: [{} as Color, [Validators.required('Color')]],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly destroyRef: DestroyRef,
        private readonly store: Store<AppState>,
    ) {}

    public ngOnInit(): void {
        this.store.dispatch(loadStatuses({ repositoryName: this.repositoryName() }));

        this.store
            .select(selectStatuses)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                map(statuses => statuses.data.find(label => label.id === this.statusId())),
                filter(status => !!status),
            )
            .subscribe(status =>
                this.form.setValue({
                    name: status.name,
                    color: status.color,
                }),
            );
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let status = {
            name: this.form.value.name!,
            color: this.form.value.color!,
        };

        this.isLoading.set(true);
        this.store.dispatch(
            updateStatus({
                repositoryName: this.repositoryName(),
                statusId: this.statusId(),
                status,
            }),
        );
    }
}
