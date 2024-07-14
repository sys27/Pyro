import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { CreateRepository, RepositoryService } from '../../../services/repository.service';

@Component({
    selector: 'repository-new',
    standalone: true,
    imports: [ReactiveFormsModule, ButtonModule, InputTextModule],
    templateUrl: './repository-new.component.html',
    styleUrls: ['./repository-new.component.css'],
})
export class RepositoryNewComponent {
    public form = this.formBuilder.nonNullable.group({
        name: [
            '',
            [Validators.required, Validators.maxLength(20), Validators.pattern(/^[a-zA-Z0-9-_]+$/)],
        ],
        description: ['', [Validators.maxLength(250)]],
        defaultBranch: ['', [Validators.required, Validators.maxLength(50)]],
    });

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly repositoryService: RepositoryService,
    ) {}

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.repositoryService
            .createRepository(this.form.value as CreateRepository)
            .subscribe(() => this.router.navigate(['/repositories', this.form.value.name]));
    }
}
