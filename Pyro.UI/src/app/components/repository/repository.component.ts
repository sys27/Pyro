import { loadRepository, reloadRepository } from '@actions/repository.actions';
import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, input, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';
import { WebSocketEvents } from '@services/web-socket.service';
import { AppState } from '@states/app.state';
import { selectIsRepositoryInitialized } from '@states/repository.state';
import { selectMessage } from '@states/web-socket.state';
import { MenuItem } from 'primeng/api';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TabMenuModule } from 'primeng/tabmenu'; // TODO: replace
import { distinctUntilChanged, filter, map, Observable } from 'rxjs';

@Component({
    selector: 'repo-list',
    imports: [AsyncPipe, ProgressSpinnerModule, RouterOutlet, TabMenuModule],
    templateUrl: './repository.component.html',
    styleUrl: './repository.component.css',
})
export class RepositoryComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly isInitialized$: Observable<boolean> = this.store.select(
        selectIsRepositoryInitialized,
    );
    private repositoryInitializedMessages$: Observable<string> = this.store.pipe(
        selectMessage(WebSocketEvents.RepositoryInitialized),
        map(message => message.payload.repositoryName),
    );
    public menu: MenuItem[] = [];

    private readonly menuTemplate: MenuItem[] = [
        { label: 'Code', icon: 'pi pi-code', routerLink: 'code' },
        { label: 'Issues', icon: 'pi pi-ticket', routerLink: 'issues' },
        { label: 'Pull Requests', icon: 'pi pi-file-import', routerLink: 'pull-requests' },
        { label: 'Settings', icon: 'pi pi-cog', routerLink: 'settings' },
    ];

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly store: Store<AppState>,
    ) {}

    public ngOnInit(): void {
        this.isInitialized$
            .pipe(takeUntilDestroyed(this.destroyRef), distinctUntilChanged())
            .subscribe(isInitialized => {
                this.menu = this.menuTemplate.map(item => ({ ...item, disabled: !isInitialized }));
            });
        this.repositoryInitializedMessages$
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                filter(repositoryName => repositoryName === this.repositoryName()),
            )
            .subscribe(() => {
                this.store.dispatch(reloadRepository({ repositoryName: this.repositoryName() }));
            });

        this.store.dispatch(loadRepository({ repositoryName: this.repositoryName() }));
    }
}
