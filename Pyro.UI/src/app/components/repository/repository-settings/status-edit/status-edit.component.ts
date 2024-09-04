import { Component, input, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { WithValidationComponent } from '@controls/with-validation/with-validation.component';
import { Color } from '@models/color';
import { IssueStatusService } from '@services/issue-status.service';
import { mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'status-edit',
    standalone: true,
    imports: [
        ButtonModule,
        ColorPickerModule,
        InputTextModule,
        ReactiveFormsModule,
        WithValidationComponent,
    ],
    templateUrl: './status-edit.component.html',
    styleUrl: './status-edit.component.css',
})
export class StatusEditComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly statusId = input.required<string>();
    public readonly form = this.formBuilder.nonNullable.group({
        name: ['', [Validators.required, Validators.maxLength(50)]],
        color: [{} as Color, [Validators.required]],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly statusService: IssueStatusService,
    ) {}

    public ngOnInit(): void {
        this.statusService
            .getStatus(this.repositoryName(), this.statusId())
            .pipe(mapErrorToNull)
            .subscribe(status => {
                if (status === null) {
                    return;
                }

                this.form.setValue({
                    name: status.name,
                    color: status.color,
                });
            });
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.isLoading.set(true);

        let status = {
            name: this.form.value.name!,
            color: this.form.value.color!,
        };

        this.statusService
            .updateStatus(this.repositoryName(), this.statusId(), status)
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