import { CommonModule } from '@angular/common';
import { Component, OnInit, input } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ListboxModule } from 'primeng/listbox';
import { MultiSelectModule } from 'primeng/multiselect';
import { Role, UpdateUser, User, UserService } from '../../services/user.service';

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

    public user: User | undefined;
    public roles: Role[] | undefined;

    public form = this.formBuilder.nonNullable.group({
        login: ['', [Validators.required, Validators.email]],
        isLocked: [false, Validators.required],
        roles: new FormControl<Role[]>([]),
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly route: ActivatedRoute,
        private readonly userService: UserService,
    ) {}

    public ngOnInit(): void {
        this.form.get('login')?.disable();

        this.userService.getRoles().subscribe(roles => (this.roles = roles));

        this.userService.getUser(this.login()).subscribe(user => {
            this.user = user;

            this.form.setValue({
                login: user.login,
                isLocked: user.isLocked,
                roles: user.roles,
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
