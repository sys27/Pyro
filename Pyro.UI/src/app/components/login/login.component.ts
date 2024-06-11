import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { PasswordModule } from 'primeng/password';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
    selector: 'login',
    standalone: true,
    imports: [
        ReactiveFormsModule,
        InputTextModule,
        PasswordModule,
        ButtonModule],
    templateUrl: './login.component.html',
    styleUrl: './login.component.css'
})
export class LoginComponent {
    public formGroup = this.formBuilder.nonNullable.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', Validators.required]
    });

    public constructor(
        private formBuilder: FormBuilder,
        private router: Router,
        private authService: AuthService,
    ) { }

    public onSubmit(): void {
        if (this.formGroup.invalid)
            return;

        let email = this.formGroup.value.email!;
        let password = this.formGroup.value.password!;

        this.authService.login(email, password);

        this.router.navigate(['/repositories']);
    }
}
