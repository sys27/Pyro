import { resetPassword } from '@actions/users.actions';
import { Component, input } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'reset-password',
    standalone: true,
    imports: [ButtonModule, InputTextModule, ReactiveFormsModule, ValidationSummaryComponent],
    templateUrl: './reset-password.component.html',
    styleUrl: './reset-password.component.css',
})
export class ResetPasswordComponent {
    public readonly form = this.formBulder.group(
        {
            token: [''],
            password: ['', [Validators.required('Password'), Validators.minLength('Password', 8)]],
            confirmPassword: [
                '',
                [
                    Validators.required('Confirm Password'),
                    Validators.minLength('Confirm Password', 8),
                ],
            ],
        },
        {
            validators: Validators.sameAs(
                ['password', 'Password'],
                ['confirmPassword', 'Confirm Password'],
            ),
        },
    );

    public token = input.required<string>();

    public constructor(
        private readonly formBulder: FormBuilder,
        private readonly store: Store<AppState>,
    ) {}

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.store.dispatch(
            resetPassword({ token: this.token(), password: this.form.value.password! }),
        );
    }
}
