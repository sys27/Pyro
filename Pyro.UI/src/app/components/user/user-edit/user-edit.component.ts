import { AsyncPipe } from '@angular/common';
import { Component, OnInit, input } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { mapErrorToEmpty, mapErrorToNull } from '@services/operators';
import { Role, UpdateUser, UserService } from '@services/user.service';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ListboxModule } from 'primeng/listbox';
import { MultiSelectModule } from 'primeng/multiselect';
import { Observable, shareReplay } from 'rxjs';

@Component({
    selector: 'user',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        CheckboxModule,
        InputTextModule,
        MultiSelectModule,
        ListboxModule,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './user-edit.component.html',
    styleUrl: './user-edit.component.css',
})
export class UserEditComponent implements OnInit {
    public readonly login = input.required<string>();
    public roles$: Observable<Role[]> | undefined;
    public readonly form = this.formBuilder.nonNullable.group({
        login: ['', [Validators.required('Login')]],
        isLocked: [false, Validators.required('Is Locked')],
        roles: new FormControl<Role[]>([], Validators.required('Roles')),
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly userService: UserService,
    ) {}

    public ngOnInit(): void {
        this.form.get('login')?.disable();

        this.roles$ = this.userService.getRoles().pipe(mapErrorToEmpty, shareReplay(1));

        this.userService
            .getUser(this.login())
            .pipe(mapErrorToNull)
            .subscribe(user => {
                this.form.patchValue({
                    login: user?.login,
                    isLocked: user?.isLocked,
                    roles: user?.roles,
                });
            });
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.userService
            .updateUser(this.login(), this.form.value as UpdateUser)
            .subscribe(() => {});
    }
}
