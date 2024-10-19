import { createRepository } from '@actions/repositories.actions';
import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { Store } from '@ngrx/store';
import { CreateRepository, RepositoryService } from '@services/repository.service';
import { WebSocketEvents } from '@services/web-socket.service';
import { AppState } from '@states/app.state';
import { selectIsNewRepositoryProcessing } from '@states/repositories.state';
import { selectMessage } from '@states/web-socket.state';
import { AutoFocusModule } from 'primeng/autofocus';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { filter, map, Observable } from 'rxjs';

@Component({
    selector: 'repository-new',
    standalone: true,
    imports: [
        AsyncPipe,
        AutoFocusModule,
        ButtonModule,
        InputTextModule,
        ReactiveFormsModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './repository-new.component.html',
    styleUrls: ['./repository-new.component.css'],
})
export class RepositoryNewComponent {
    public readonly form = this.formBuilder.nonNullable.group({
        name: [
            '',
            [
                Validators.required('Name'),
                Validators.maxLength('Name', 20),
                Validators.pattern('Name', /^[a-zA-Z0-9-_]+$/),
            ],
            [
                Validators.exists(
                    value => `The '${value}' repository already exists`,
                    value => this.repositoryService.getRepository(value),
                ),
            ],
        ],
        description: ['', [Validators.maxLength('Description', 250)]],
        defaultBranch: [
            '',
            [Validators.required('Default Branch'), Validators.maxLength('Default Branch', 50)],
        ],
    });
    public readonly isLoading$: Observable<boolean> = this.store.select(
        selectIsNewRepositoryProcessing,
    );
    private repositoryInitializedMessages$: Observable<string> = this.store.pipe(
        selectMessage(WebSocketEvents.RepositoryInitialized),
        map(message => message.payload.repositoryName),
    );

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly store: Store<AppState>,
        private readonly repositoryService: RepositoryService,
    ) {}

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let repository: CreateRepository = {
            name: this.form.value.name!,
            description: this.form.value.description!,
            defaultBranch: this.form.value.defaultBranch!,
        };

        this.repositoryInitializedMessages$
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                filter(repositoryName => repositoryName === this.form.value.name),
            )
            .subscribe(repositoryName => this.router.navigate(['/repositories', repositoryName]));
        this.store.dispatch(createRepository({ repository }));
    }
}
