import { loadUser, updateUser } from '@actions/users.actions';
import { Component, input, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { DataSourceDirective } from '@directives/data-source.directive';
import { Store } from '@ngrx/store';
import { Role, UpdateUser } from '@services/user.service';
import { AppState } from '@states/app.state';
import { DataSourceState } from '@states/data-source.state';
import { selectRolesFeature } from '@states/roles.state';
import { selectSelectedUser } from '@states/users.state';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ListboxModule } from 'primeng/listbox';
import { MultiSelectModule } from 'primeng/multiselect';
import { filter, Observable } from 'rxjs';

@Component({
    selector: 'user',
    imports: [
        ButtonModule,
        CheckboxModule,
        DataSourceDirective,
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
    public roles$: Observable<DataSourceState<Role>> = this.store.select(selectRolesFeature);
    public readonly form = this.formBuilder.nonNullable.group({
        login: ['', [Validators.required('Login')]],
        roles: new FormControl<Role[]>([], Validators.required('Roles')),
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly store: Store<AppState>,
    ) {
        this.store
            .select(selectSelectedUser)
            .pipe(filter(user => !!user))
            .subscribe(user => {
                this.form.patchValue({
                    login: user.login,
                    roles: user.roles,
                });
            });
    }

    public ngOnInit(): void {
        this.store.dispatch(loadUser({ login: this.login() }));

        this.form.get('login')?.disable();
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let user: UpdateUser = {
            roles: this.form.value.roles!,
        };
        this.store.dispatch(updateUser({ login: this.login(), user }));
    }
}
