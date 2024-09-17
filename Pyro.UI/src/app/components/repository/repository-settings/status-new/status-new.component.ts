import { Component, Injector, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Color } from '@models/color';
import { IssueStatusService } from '@services/issue-status.service';
import { createErrorHandler } from '@services/operators';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';
import { filter, finalize } from 'rxjs';

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
        private readonly injector: Injector,
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
            .pipe(
                createErrorHandler(this.injector),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe(() => {
                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
