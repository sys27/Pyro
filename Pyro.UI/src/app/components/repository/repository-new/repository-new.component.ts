import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { NotificationEvent, NotificationService } from '@services/notification.service';
import { CreateRepository, RepositoryService } from '@services/repository.service';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { filter, Observable, Subject, switchMap, takeUntil } from 'rxjs';

@Component({
    selector: 'repository-new',
    standalone: true,
    imports: [ReactiveFormsModule, ButtonModule, InputTextModule],
    templateUrl: './repository-new.component.html',
    styleUrls: ['./repository-new.component.css'],
})
export class RepositoryNewComponent implements OnInit, OnDestroy {
    public form = this.formBuilder.nonNullable.group({
        name: [
            '',
            [Validators.required, Validators.maxLength(20), Validators.pattern(/^[a-zA-Z0-9-_]+$/)],
        ],
        description: ['', [Validators.maxLength(250)]],
        defaultBranch: ['', [Validators.required, Validators.maxLength(50)]],
    });
    public isLoading: boolean = false;

    private readonly destroy$: Subject<void> = new Subject<void>();
    private repositoryInitialized$: Observable<string> | undefined;

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly repositoryService: RepositoryService,
        private readonly notificationService: NotificationService,
    ) {}

    public ngOnInit(): void {
        this.repositoryInitialized$ = this.notificationService
            .on<string>(NotificationEvent.RepositoryInitialized)
            .pipe(takeUntil(this.destroy$));
    }

    public ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.isLoading = true;

        this.repositoryService
            .createRepository(this.form.value as CreateRepository)
            .pipe(
                switchMap(() => this.repositoryInitialized$!),
                filter(name => name === this.form.value.name),
            )
            .subscribe(name => {
                this.isLoading = false;
                this.router.navigate(['/repositories', this.form.value.name]);
            });
    }
}
