import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { mapErrorToNull } from '../../services/operators';
import { ProfileService, UpdateProfile } from '../../services/profile.service';

@Component({
    selector: 'profile',
    standalone: true,
    imports: [ReactiveFormsModule, ButtonModule, InputTextModule],
    templateUrl: './profile.component.html',
    styleUrl: './profile.component.css',
})
export class ProfileComponent implements OnInit {
    public form = this.formBuilder.group({
        email: ['', Validators.required, Validators.email],
        name: [''],
        status: [''],
    });

    public constructor(
        private formBuilder: FormBuilder,
        private profileService: ProfileService,
    ) {}

    public ngOnInit(): void {
        this.form.get('email')!.disable();

        this.profileService
            .getProfile()
            .pipe(mapErrorToNull)
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

        this.profileService.updateProfile(this.form.value as UpdateProfile).subscribe(() => {});
    }
}
