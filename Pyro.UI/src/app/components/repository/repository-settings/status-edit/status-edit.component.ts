import { Component, Injector, input, OnInit, signal } from '@angular/core';
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
import { finalize } from 'rxjs';

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
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly statusService: IssueStatusService,
    ) {}

    public ngOnInit(): void {
        this.statusService
            .getStatus(this.repositoryName(), this.statusId())
            .pipe(createErrorHandler(this.injector))
            .subscribe(status => {
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
            .pipe(
                createErrorHandler(this.injector),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe(() => {
                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
