import { loadRoles } from '@actions/roles.actions';
import { createUser } from '@actions/users.actions';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { DataSourceDirective } from '@directives/data-source.directive';
import { Store } from '@ngrx/store';
import { CreateUser, Role, UserService } from '@services/user.service';
import { AppState } from '@states/app.state';
import { DataSourceState } from '@states/data-source.state';
import { selectRolesFeature } from '@states/roles.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ListboxModule } from 'primeng/listbox';
import { MultiSelectModule } from 'primeng/multiselect';
import { Observable } from 'rxjs';

@Component({
    selector: 'user-new',
    imports: [
        AutoFocusModule,
        ButtonModule,
        CheckboxModule,
        DataSourceDirective,
        InputTextModule,
        MultiSelectModule,
        ListboxModule,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './user-new.component.html',
    styleUrl: './user-new.component.css',
})
export class UserNewComponent implements OnInit {
    public roles$: Observable<DataSourceState<Role>> = this.store.select(selectRolesFeature);

    public readonly form = this.formBuilder.nonNullable.group({
        login: [
            '',
            [
                Validators.required('Login'),
                Validators.maxLength('Login', 32),
                Validators.pattern('Login', /^[a-zA-Z0-9_\\-]+$/),
            ],
            [
                Validators.exists(
                    value => `The user with login '${value}' already exists`,
                    value => this.userService.getUser(value),
                ),
            ],
        ],
        email: [
            '',
            [
                Validators.required('Email'),
                Validators.maxLength('Email', 50),
                Validators.email('Email'),
            ],
        ],
        roles: new FormControl<Role[]>([], Validators.required('Roles')),
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly store: Store<AppState>,
        private readonly userService: UserService,
    ) {}

    public ngOnInit(): void {
        this.store.dispatch(loadRoles());
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let user: CreateUser = {
            login: this.form.value.login!,
            email: this.form.value.email!,
            roles: this.form.value.roles!,
        };

        this.store.dispatch(createUser({ user }));
    }
}
