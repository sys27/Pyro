import { Component, Injector, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Color } from '@models/color';
import { LabelService } from '@services/label.service';
import { createErrorHandler } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';
import { finalize } from 'rxjs';

@Component({
    selector: 'label-edit',
    standalone: true,
    imports: [
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
export class LabelEditComponent {
    public readonly repositoryName = input.required<string>();
    public readonly labelId = input.required<string>();
    public readonly form = this.formBuilder.nonNullable.group({
        name: ['', [Validators.required('Name'), Validators.maxLength('Name', 50)]],
        color: [{} as Color, [Validators.required('Name')]],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly labelService: LabelService,
    ) {}

    public ngOnInit(): void {
        this.labelService
            .getLabel(this.repositoryName(), this.labelId())
            .pipe(createErrorHandler(this.injector))
            .subscribe(label => {
                this.form.setValue({
                    name: label.name,
                    color: label.color,
                });
            });
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.isLoading.set(true);

        let label = {
            name: this.form.value.name!,
            color: this.form.value.color!,
        };

        this.labelService
            .updateLabel(this.repositoryName(), this.labelId(), label)
            .pipe(
                createErrorHandler(this.injector),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe(() => {
                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
