import { Component, Injector, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { ObservableOptionsDirective } from '@directives/observable-options.directive';
import { createErrorHandler } from '@services/operators';
import { CreateUser, Role, User, UserService } from '@services/user.service';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ListboxModule } from 'primeng/listbox';
import { MultiSelectModule } from 'primeng/multiselect';
import { Observable } from 'rxjs';

@Component({
    selector: 'user-new',
    standalone: true,
    imports: [
        AutoFocusModule,
        ButtonModule,
        CheckboxModule,
        InputTextModule,
        MultiSelectModule,
        ListboxModule,
        ObservableOptionsDirective,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './user-new.component.html',
    styleUrl: './user-new.component.css',
})
export class UserNewComponent implements OnInit {
    public user: User | undefined;
    public roles$: Observable<Role[]> | undefined;

    public readonly form = this.formBuilder.nonNullable.group({
        login: [
            '',
            [Validators.required('Login'), Validators.maxLength('Login', 32)],
            [
                Validators.exists(
                    value => `The user with login '${value}' already exists`,
                    value => this.userService.getUser(value),
                ),
            ],
        ],
        password: ['', [Validators.required('Password'), Validators.minLength('Password', 8)]],
        roles: new FormControl<Role[]>([], Validators.required('Roles')),
    });

    public constructor(
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly userService: UserService,
    ) {}

    public ngOnInit(): void {
        this.roles$ = this.userService.getRoles().pipe(createErrorHandler(this.injector));
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.userService
            .createUser(this.form.value as CreateUser)
            .pipe(createErrorHandler(this.injector))
            .subscribe(() => this.router.navigate(['/users']));
    }
}
