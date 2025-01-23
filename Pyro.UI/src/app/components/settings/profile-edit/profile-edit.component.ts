import { loadProfile, updateProfile } from '@actions/profile.actions';
import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Store } from '@ngrx/store';
import { UpdateProfile } from '@services/profile.service';
import { AppState } from '@states/app.state';
import { selectCurrentProfile } from '@states/profile.state';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { filter } from 'rxjs';

@Component({
    selector: 'profile-edit',
    imports: [ButtonModule, InputTextModule, ReactiveFormsModule, ValidationSummaryComponent],
    templateUrl: './profile-edit.component.html',
    styleUrl: './profile-edit.component.css',
})
export class ProfileEditComponent implements OnInit {
    public readonly form = this.formBuilder.group({
        displayName: ['', [Validators.required('Name'), Validators.maxLength('Name', 50)]],
        email: ['', [Validators.required('Email'), Validators.email('Email')]],
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly destroyRef: DestroyRef,
        private readonly store: Store<AppState>,
    ) {}

    public ngOnInit(): void {
        this.store.dispatch(loadProfile());

        this.store
            .select(selectCurrentProfile)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                filter(profile => !!profile),
            )
            .subscribe(profile => {
                this.form.patchValue({
                    displayName: profile.displayName,
                    email: profile.email,
                });
            });
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let profile: UpdateProfile = {
            displayName: this.form.value.displayName!,
            email: this.form.value.email!,
        };

        this.store.dispatch(updateProfile({ profile }));
    }
}
