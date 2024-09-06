import { Component, Injector, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { AccessTokenService, CreateAccessToken } from '@services/access-token.service';
import { createErrorHandler } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputTextModule } from 'primeng/inputtext';
import { filter } from 'rxjs';

@Component({
    selector: 'access-token-new',
    standalone: true,
    imports: [
        ButtonModule,
        CalendarModule,
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
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly service: AccessTokenService,
    ) {}

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.service
            .createAccessToken(this.form.value as CreateAccessToken)
            .pipe(createErrorHandler(this.injector))
            .subscribe(response => {
                this.form.disable();
                this.isViewMode.set(true);
                this.form.controls.token.setValue(response.token);
            });
    }

    public copyToken() {
        let token = this.form.controls.token.value;
        navigator.clipboard.writeText(token);
    }
}
