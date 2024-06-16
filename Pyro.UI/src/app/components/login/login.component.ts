import { Location } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'login',
    standalone: true,
    imports: [ReactiveFormsModule, InputTextModule, PasswordModule, ButtonModule],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css',
})
export class LoginComponent {
    public formGroup = this.formBuilder.nonNullable.group({
        login: ['', [Validators.required, Validators.email]],
        password: ['', Validators.required],
    });

    public constructor(
        private formBuilder: FormBuilder,
        private location: Location,
        private authService: AuthService,
    ) {}

    public onSubmit(): void {
        if (this.formGroup.invalid) return;

        let login = this.formGroup.value.login!;
        let password = this.formGroup.value.password!;

        this.authService.login(login, password).subscribe(currentUser => {
            if (!currentUser) return;

            this.location.back();
        });
    }
}
