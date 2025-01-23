import { loadLabels, updateLabel } from '@actions/repository-labels.actions';
import { Component, DestroyRef, input, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Color } from '@models/color';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { selectLabels } from '@states/repository.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';
import { filter, map } from 'rxjs';

@Component({
    selector: 'label-edit',
    imports: [
        AutoFocusModule,
        ButtonModule,
        ColorPickerModule,
        InputTextModule,
        ReactiveFormsModule,
        RouterLink,
        ValidationSummaryComponent,
    ],
    templateUrl: './label-edit.component.html',
    styleUrl: './label-edit.component.css',
})
export class LabelEditComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly labelId = input.required<string>();
    public readonly form = this.formBuilder.nonNullable.group({
        name: ['', [Validators.required('Name'), Validators.maxLength('Name', 50)]],
        color: [{} as Color, [Validators.required('Name')]],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly destroyRef: DestroyRef,
        private readonly store: Store<AppState>,
    ) {}

    public ngOnInit(): void {
        this.store.dispatch(loadLabels({ repositoryName: this.repositoryName() }));

        this.store
            .select(selectLabels)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                map(labels => labels.data.find(label => label.id === this.labelId())),
                filter(label => !!label),
            )
            .subscribe(label =>
                this.form.setValue({
                    name: label.name,
                    color: label.color,
                }),
            );
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let label = {
            name: this.form.value.name!,
            color: this.form.value.color!,
        };

        this.isLoading.set(true);
        this.store.dispatch(
            updateLabel({
                repositoryName: this.repositoryName(),
                labelId: this.labelId(),
                label,
            }),
        );
    }
}
