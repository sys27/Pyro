import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, Injector, OnDestroy, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterModule } from '@angular/router';
import { PaginatorComponent, PaginatorState } from '@controls/paginator/paginator.component';
import { PyroPermissions } from '@models/pyro-permissions';
import { AuthService } from '@services/auth.service';
import { createErrorHandler } from '@services/operators';
import { User, UserItem, UserService } from '@services/user.service';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { BehaviorSubject, map, Observable, switchMap } from 'rxjs';

@Component({
    selector: 'user-list',
    standalone: true,
    imports: [AsyncPipe, ButtonModule, PaginatorComponent, RouterModule, TableModule],
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
})
export class UserListComponent implements OnInit, OnDestroy {
    public readonly users = signal<UserItem[]>([]);
    private readonly refreshUsers$ = new BehaviorSubject<void>(undefined);
    public hasEditPermission$: Observable<boolean> | undefined;

    public constructor(
        private readonly injector: Injector,
        private readonly destroyRef: DestroyRef,
        private readonly userService: UserService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.hasEditPermission$ = this.authService.currentUser.pipe(
            takeUntilDestroyed(this.destroyRef),
            map(user => user?.hasPermission(PyroPermissions.UserEdit) ?? false),
        );
    }

    public ngOnDestroy(): void {
        this.refreshUsers$.complete();
    }

    public paginatorLoader = (state: PaginatorState): Observable<UserItem[]> => {
        return this.refreshUsers$.pipe(
            switchMap(() => this.userService.getUsers(state.before, state.after)),
        );
    };

    public paginatorOffsetSelector(item: UserItem): string {
        return item.login;
    }

    public paginatorDataChanged(items: UserItem[]): void {
        this.users.set(items);
    }

    public lockUser(user: User): void {
        this.userService
            .lockUser(user.login)
            .pipe(createErrorHandler(this.injector))
            .subscribe(() => this.refreshUsers$.next());
    }

    public unlockUser(user: User): void {
        this.userService
            .unlockUser(user.login)
            .pipe(createErrorHandler(this.injector))
            .subscribe(() => this.refreshUsers$.next());
    }

    public isCurrentUser(user: User): Observable<boolean> {
        return this.authService.currentUser.pipe(
            takeUntilDestroyed(this.destroyRef),
            map(currentUser => currentUser?.login === user.login),
        );
    }
}
