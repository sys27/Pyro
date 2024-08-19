import { Component, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Color } from '@models/color';
import { LabelService } from '@services/label.service';
import { mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'label-edit',
    standalone: true,
    imports: [ButtonModule, ColorPickerModule, InputTextModule, ReactiveFormsModule, RouterLink],
    templateUrl: './label-edit.component.html',
    styleUrl: './label-edit.component.css',
})
export class LabelEditComponent {
    public readonly repositoryName = input.required<string>();
    public readonly labelId = input.required<string>();
    public readonly form = this.formBuilder.nonNullable.group({
        name: ['', [Validators.required, Validators.maxLength(50)]],
        color: [{} as Color, [Validators.required]],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly labelService: LabelService,
    ) {}

    public ngOnInit(): void {
        this.labelService
            .getLabel(this.repositoryName(), this.labelId())
            .pipe(mapErrorToNull)
            .subscribe(label => {
                if (label === null) {
                    return;
                }

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
            .pipe(mapErrorToNull)
            .subscribe(response => {
                if (response === null) {
                    this.isLoading.set(false);
                    return;
                }

                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
