import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { createErrorHandler } from '@services/operators';
import { ProfileService, UpdateProfile } from '@services/profile.service';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'profile',
    standalone: true,
    imports: [ButtonModule, InputTextModule, ReactiveFormsModule, ValidationSummaryComponent],
    templateUrl: './profile.component.html',
    styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
    public readonly form = this.formBuilder.group({
        email: [
            '',
            [
                Validators.required('Email'),
                Validators.email('Email'),
                Validators.maxLength('Email', 150),
            ],
        ],
        name: ['', [Validators.required('Name'), Validators.maxLength('Name', 50)]],
        status: ['', [Validators.maxLength('Status', 150)]],
    });

    public constructor(
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly profileService: ProfileService,
    ) {}

    public ngOnInit(): void {
        this.form.get('email')!.disable();

        this.profileService
            .getProfile()
            .pipe(createErrorHandler(this.injector))
            .subscribe(profile => {
                this.form.patchValue({
                    email: profile?.email,
                    name: profile?.name,
                    status: profile?.status,
                });
            });
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.profileService
            .updateProfile(this.form.value as UpdateProfile)
            .pipe(createErrorHandler(this.injector))
            .subscribe(() => {});
    }
}
