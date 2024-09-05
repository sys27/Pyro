import { AsyncPipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { mapErrorToEmpty } from '@services/operators';
import { CreateUser, Role, User, UserService } from '@services/user.service';
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
        AsyncPipe,
        ButtonModule,
        CheckboxModule,
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
    public user: User | undefined;
    public roles$: Observable<Role[]> | undefined;

    public readonly form = this.formBuilder.nonNullable.group({
        login: ['', [Validators.required('Login'), Validators.maxLength('Login', 32)]],
        password: ['', [Validators.required('Password'), Validators.minLength('Password', 8)]],
        roles: new FormControl<Role[]>([], Validators.required('Roles')),
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly userService: UserService,
    ) {}

    public ngOnInit(): void {
        this.roles$ = this.userService.getRoles().pipe(mapErrorToEmpty);
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.userService
            .createUser(this.form.value as CreateUser)
            .subscribe(() => this.router.navigate(['/users']));
    }
}
