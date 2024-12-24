import { Component, input } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { ActivateUser, UserService } from '@services/user.service';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'activate',
    imports: [ButtonModule, InputTextModule, ReactiveFormsModule, ValidationSummaryComponent],
    templateUrl: './activate.component.html',
    styleUrl: './activate.component.css',
})
export class ActivateComponent {
    public token = input.required<string>();

    public form = this.formBuilder.group(
        {
            password: ['', [Validators.required('Password'), Validators.minLength('Password', 8)]],
            confirmPassword: [
                '',
                [Validators.required('Confirm Password'), Validators.minLength('Password', 8)],
            ],
        },
        {
            validators: Validators.sameAs(
                ['password', 'Password'],
                ['confirmPassword', 'Confirm Password'],
            ),
        },
    );

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly userService: UserService,
    ) {}

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let command: ActivateUser = {
            token: this.token(),
            password: this.form.value.password!,
        };
        this.userService.activate(command).subscribe(() => {
            this.router.navigate(['/login']);
        });
    }
}
