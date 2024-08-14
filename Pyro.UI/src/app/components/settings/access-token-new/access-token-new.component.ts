import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AccessTokenService, CreateAccessToken } from '@services/access-token.service';
import { mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'access-token-new',
    standalone: true,
    imports: [ButtonModule, CalendarModule, InputTextModule, ReactiveFormsModule],
    templateUrl: './access-token-new.component.html',
    styleUrls: ['./access-token-new.component.css'],
})
export class AccessTokenNewComponent {
    public form = this.formBuilder.nonNullable.group({
        name: ['', Validators.compose([Validators.required, Validators.maxLength(50)])],
        expiresAt: [new Date(), Validators.required],
        token: [''],
    });
    public minDate: Date = new Date();
    public isViewMode = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly service: AccessTokenService,
    ) {}

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.service
            .createAccessToken(this.form.value as CreateAccessToken)
            .pipe(mapErrorToNull)
            .subscribe(response => {
                if (response === null) {
                    return;
                }

                this.form.disable();
                this.isViewMode.set(true);
                this.form.controls.token.setValue(response.token);
            });
    }
}
