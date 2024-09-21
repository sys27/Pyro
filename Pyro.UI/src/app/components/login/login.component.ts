import { loginAction } from '@actions/auth.actions';
import { Component, input } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';

@Component({
    selector: 'login',
    standalone: true,
    imports: [
        AutoFocusModule,
        ButtonModule,
        InputTextModule,
        PasswordModule,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css',
})
export class LoginComponent {
    public readonly returnUrl = input<string>('/');
    public readonly formGroup = this.formBuilder.nonNullable.group({
        login: ['', Validators.required('Login')],
        password: ['', Validators.required('Password')],
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly store: Store<AppState>,
    ) {}

    public onSubmit(): void {
        if (this.formGroup.invalid) {
            return;
        }

        let login = this.formGroup.value.login!;
        let password = this.formGroup.value.password!;

        this.store.dispatch(loginAction({ login, password, returnUrl: this.returnUrl() }));
    }
}
