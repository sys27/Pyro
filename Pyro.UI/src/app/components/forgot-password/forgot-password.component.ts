import { forgotPassword } from '@actions/users.actions';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'forgot-password',
    imports: [ButtonModule, InputTextModule, ReactiveFormsModule, ValidationSummaryComponent],
    templateUrl: './forgot-password.component.html',
    styleUrl: './forgot-password.component.css',
})
export class ForgotPasswordComponent {
    public form = this.formBuilder.group({
        login: ['', Validators.required('Login')],
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly store: Store<AppState>,
    ) {}

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.store.dispatch(forgotPassword({ login: this.form.value.login! }));
    }
}
