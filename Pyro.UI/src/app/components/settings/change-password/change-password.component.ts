import { changePassword } from '@actions/users.actions';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'change-password',
    standalone: true,
    imports: [ButtonModule, InputTextModule, ReactiveFormsModule, ValidationSummaryComponent],
    templateUrl: './change-password.component.html',
    styleUrl: './change-password.component.css',
})
export class ChangePasswordComponent {
    public form = this.formBuilder.group(
        {
            oldPassword: ['', [Validators.required('Old Password')]],
            newPassword: [
                '',
                [Validators.required('New Password'), Validators.minLength('New Password', 8)],
            ],
            confirmPassword: [
                '',
                [Validators.required('Confirm Password'), Validators.minLength('New Password', 8)],
            ],
        },
        {
            validators: Validators.sameAs(
                ['newPassword', 'New Password'],
                ['confirmPassword', 'Confirm Password'],
            ),
        },
    );

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly store: Store<AppState>,
    ) {}

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let oldPassword = this.form.value.oldPassword!;
        let newPassword = this.form.value.newPassword!;
        this.store.dispatch(changePassword({ oldPassword, newPassword }));

        this.form.reset();
    }
}
