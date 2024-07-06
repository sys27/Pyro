import { CommonModule } from '@angular/common';
import { Component, OnInit, input } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ListboxModule } from 'primeng/listbox';
import { MultiSelectModule } from 'primeng/multiselect';
import { mapErrorToEmpty, mapErrorToNull } from '../../../services/operators';
import { Role, UpdateUser, UserService } from '../../../services/user.service';

@Component({
    selector: 'user',
    standalone: true,
    imports: [
        CommonModule,
        ReactiveFormsModule,
        InputTextModule,
        MultiSelectModule,
        ListboxModule,
        ButtonModule,
        CheckboxModule,
    ],
    templateUrl: './user-edit.component.html',
    styleUrl: './user-edit.component.css',
})
export class UserEditComponent implements OnInit {
    public login = input.required<string>();

    public roles: Role[] | undefined;

    public form = this.formBuilder.nonNullable.group({
        login: ['', [Validators.required]],
        isLocked: [false, Validators.required],
        roles: new FormControl<Role[]>([]),
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly userService: UserService,
    ) {}

    public ngOnInit(): void {
        this.form.get('login')?.disable();

        this.userService
            .getRoles()
            .pipe(mapErrorToEmpty)
            .subscribe(roles => (this.roles = roles));

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
