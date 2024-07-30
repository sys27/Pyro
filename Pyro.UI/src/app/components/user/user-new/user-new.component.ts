import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ListboxModule } from 'primeng/listbox';
import { MultiSelectModule } from 'primeng/multiselect';
import { mapErrorToEmpty } from '@services/operators';
import { CreateUser, Role, User, UserService } from '@services/user.service';

@Component({
    selector: 'user-new',
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
    templateUrl: './user-new.component.html',
    styleUrl: './user-new.component.css',
})
export class UserNewComponent implements OnInit {
    public user: User | undefined;
    public roles: Role[] | undefined;

    public form = this.formBuilder.nonNullable.group({
        login: ['', [Validators.required, Validators.maxLength(32)]],
        password: ['', [Validators.required, Validators.minLength(8)]],
        roles: new FormControl<Role[]>([], Validators.required),
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly userService: UserService,
    ) {}

    public ngOnInit(): void {
        this.userService
            .getRoles()
            .pipe(mapErrorToEmpty)
            .subscribe(roles => (this.roles = roles));
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
