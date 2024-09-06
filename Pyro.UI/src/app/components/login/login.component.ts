import { Component, Injector, input } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { AuthService } from '@services/auth.service';
import { createErrorHandler } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';

@Component({
    selector: 'login',
    standalone: true,
    imports: [
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
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly authService: AuthService,
    ) {}

    private static sanitizeReturnUrl(returnUrl: string): string {
        returnUrl ??= '/';

        const currentUrl = window.location.origin;

        let url = new URL(returnUrl, currentUrl);
        if (url.origin !== currentUrl) {
            return '/';
        }

        return url.pathname;
    }

    public onSubmit(): void {
        if (this.formGroup.invalid) {
            return;
        }

        let login = this.formGroup.value.login!;
        let password = this.formGroup.value.password!;

        this.authService
            .login(login, password)
            .pipe(createErrorHandler(this.injector))
            .subscribe(currentUser => {
                if (!currentUser) {
                    return;
                }

                this.router.navigateByUrl(LoginComponent.sanitizeReturnUrl(this.returnUrl()));
            });
    }
}
