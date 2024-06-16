import { CommonModule } from '@angular/common';
import { Component, OnInit, input } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { CheckboxModule } from 'primeng/checkbox';
import { InputTextModule } from 'primeng/inputtext';
import { ListboxModule } from 'primeng/listbox';
import { MultiSelectModule } from 'primeng/multiselect';
import { CreateUser, Role, User, UserService } from '../../services/user.service';

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
    public email = input.required<string>();

    public user: User | undefined;
    public roles: Role[] | undefined;

    public form = this.formBuilder.nonNullable.group({
        email: ['', [Validators.required, Validators.email]],
        password: ['', Validators.required],
        roles: new FormControl<Role[]>([]),
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly route: ActivatedRoute,
        private readonly userService: UserService,
    ) {}

    public ngOnInit(): void {
        this.userService.getRoles().subscribe(roles => (this.roles = roles));
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.userService.createUser(this.form.value as CreateUser).subscribe(() => {});
    }
}
