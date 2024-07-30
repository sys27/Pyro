import { Component, input } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '@services/auth.service';

@Component({
    selector: 'login',
    standalone: true,
    imports: [ReactiveFormsModule, InputTextModule, PasswordModule, ButtonModule],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css',
})
export class LoginComponent {
    public returnUrl = input<string>('/');

    public formGroup = this.formBuilder.nonNullable.group({
        login: ['', [Validators.required]],
        password: ['', Validators.required],
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly authService: AuthService,
    ) {}

    private sanitizeReturnUrl(returnUrl: string): string {
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

        this.authService.login(login, password).subscribe(currentUser => {
            if (!currentUser) {
                return;
            }

            this.router.navigateByUrl(this.sanitizeReturnUrl(this.returnUrl()));
        });
    }
}
