import { Component, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Color } from '@models/color';
import { LabelService } from '@services/label.service';
import { mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'label-new',
    standalone: true,
    imports: [ButtonModule, ColorPickerModule, InputTextModule, ReactiveFormsModule],
    templateUrl: './label-new.component.html',
    styleUrl: './label-new.component.css',
})
export class LabelNewComponent {
    public readonly repositoryName = input.required<string>();
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
            .createLabel(this.repositoryName(), label)
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
