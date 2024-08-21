import { Component, OnDestroy, OnInit, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { WithValidationComponent } from '@controls/with-validation/with-validation.component';
import { NotificationEvent, NotificationService } from '@services/notification.service';
import { CreateRepository, RepositoryService } from '@services/repository.service';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { filter, finalize, Observable, Subject, switchMap, takeUntil } from 'rxjs';

@Component({
    selector: 'repository-new',
    standalone: true,
    imports: [ButtonModule, InputTextModule, ReactiveFormsModule, WithValidationComponent],
    templateUrl: './repository-new.component.html',
    styleUrls: ['./repository-new.component.css'],
})
export class RepositoryNewComponent implements OnInit, OnDestroy {
    public readonly form = this.formBuilder.nonNullable.group({
        name: [
            '',
            [Validators.required, Validators.maxLength(20), Validators.pattern(/^[a-zA-Z0-9-_]+$/)],
        ],
        description: ['', [Validators.maxLength(250)]],
        defaultBranch: ['', [Validators.required, Validators.maxLength(50)]],
    });
    public readonly isLoading = signal<boolean>(false);
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

        this.isLoading.set(true);

        this.repositoryService
            .createRepository(this.form.value as CreateRepository)
            .pipe(
                switchMap(() => this.repositoryInitialized$!),
                filter(name => name === this.form.value.name),
                finalize(() => this.isLoading.set(false)),
            )
            .subscribe(name => {
                this.router.navigate(['/repositories', name]);
            });
    }
}
