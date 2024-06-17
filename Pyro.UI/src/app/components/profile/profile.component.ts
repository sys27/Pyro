import { Component, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
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
        name: [''],
        status: [''],
    });

    public constructor(
        private formBuilder: FormBuilder,
        private profileService: ProfileService,
    ) {}

    public ngOnInit(): void {
        this.profileService.getProfile().subscribe(profile => {
            this.form.patchValue({
                name: profile.name,
                status: profile.status,
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
