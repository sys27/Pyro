import { Component, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Color } from '@models/color';
import { IssueStatusService } from '@services/issue-status.service';
import { mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'status-new',
    standalone: true,
    imports: [
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
        name: ['', [Validators.required('Name'), Validators.maxLength('Name', 50)]],
        color: [{} as Color, [Validators.required('Color')]],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly statusService: IssueStatusService,
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

        this.statusService
            .createStatus(this.repositoryName(), label)
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
