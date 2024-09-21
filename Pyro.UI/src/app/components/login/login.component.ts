import { loginAction } from '@actions/auth.actions';
import { Component, DestroyRef, input } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { isLoggedInSelector } from '@states/auth.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { distinctUntilChanged } from 'rxjs';

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
        private readonly destroyRef: DestroyRef,
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly store: Store<AppState>,
    ) {
        this.store
            .select(isLoggedInSelector)
            .pipe(distinctUntilChanged(), takeUntilDestroyed(this.destroyRef))
            .subscribe(isLoggedIn => {
                if (isLoggedIn) {
                    this.router.navigateByUrl(LoginComponent.sanitizeReturnUrl(this.returnUrl()));
                }
            });
    }

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

        this.store.dispatch(loginAction({ login, password }));
    }
}
