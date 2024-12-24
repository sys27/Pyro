import { createAccessToken } from '@actions/access-tokens.actions';
import { Component, DestroyRef, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Store } from '@ngrx/store';
import { AccessTokenService, CreateAccessToken } from '@services/access-token.service';
import { AppState } from '@states/app.state';
import { selectSelectedAccessToken } from '@states/profile.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { DatePicker } from 'primeng/datepicker';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputTextModule } from 'primeng/inputtext';
import { filter } from 'rxjs';

@Component({
    selector: 'access-token-new',
    imports: [
        AutoFocusModule,
        ButtonModule,
        DatePicker,
        InputGroupModule,
        InputTextModule,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './access-token-new.component.html',
    styleUrls: ['./access-token-new.component.css'],
})
export class AccessTokenNewComponent {
    public readonly form = this.formBuilder.nonNullable.group({
        name: [
            '',
            [Validators.required('Name'), Validators.maxLength('Name', 50)],
            [
                Validators.exists(
                    value => `The access token with name '${value}' already exists`,
                    value =>
                        this.service
                            .getAccessTokens(value)
                            .pipe(filter(token => token.some(x => x.name == value))),
                ),
            ],
        ],
        expiresAt: [new Date(), Validators.required('Expires At')],
        token: [''],
    });
    public readonly minDate: Date = new Date();
    public readonly isViewMode = signal<boolean>(false);

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly formBuilder: FormBuilder,
        private readonly store: Store<AppState>,
        private readonly service: AccessTokenService,
    ) {
        this.store
            .select(selectSelectedAccessToken)
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                filter(accessToken => !!accessToken),
            )
            .subscribe(accessToken => {
                this.form.disable();
                this.isViewMode.set(true);
                this.form.controls.token.setValue(accessToken.token);
            });
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let accessToken: CreateAccessToken = {
            name: this.form.controls.name.value,
            expiresAt: this.form.controls.expiresAt.value,
        };

        this.store.dispatch(createAccessToken({ accessToken }));
    }

    public copyToken() {
        let token = this.form.controls.token.value;
        navigator.clipboard.writeText(token);
    }
}
