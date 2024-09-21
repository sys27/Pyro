import {
    loadUsers,
    lockUser,
    unlockUser,
    usersNextPage,
    usersPreviousPage,
} from '@actions/users.actions';
import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterModule } from '@angular/router';
import { PaginatorComponent } from '@controls/paginator/paginator.component';
import { DataSourceDirective } from '@directives/data-source.directive';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import { User, UserItem } from '@services/user.service';
import { AppState } from '@states/app.state';
import { currentUserSelector, hasPermissionSelector } from '@states/auth.state';
import { DataSourceState } from '@states/data-source.state';
import { currentPageSelector, hasNextSelector, hasPreviousSelector } from '@states/paged.state';
import { usersSelector } from '@states/users.state';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { map, Observable } from 'rxjs';

@Component({
    selector: 'user-list',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        DataSourceDirective,
        PaginatorComponent,
        RouterModule,
        TableModule,
    ],
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
})
export class UserListComponent implements OnInit {
    public readonly users$: Observable<DataSourceState<UserItem>> = this.store.select(
        currentPageSelector(usersSelector),
    );
    public readonly isPreviousEnabled$: Observable<boolean> = this.store.select(
        hasPreviousSelector(usersSelector),
    );
    public readonly isNextEnabled$: Observable<boolean> = this.store.select(
        hasNextSelector(usersSelector),
    );
    public readonly hasEditPermission$: Observable<boolean> = this.store.select(
        hasPermissionSelector(PyroPermissions.UserEdit),
    );

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly store: Store<AppState>,
    ) {}

    public ngOnInit(): void {
        this.store.dispatch(loadUsers());
    }

    public lockUserHandler(user: User): void {
        this.store.dispatch(lockUser({ login: user.login }));
    }

    public unlockUserHandler(user: User): void {
        this.store.dispatch(unlockUser({ login: user.login }));
    }

    public isCurrentUser(user: User): Observable<boolean> {
        return this.store.select(currentUserSelector).pipe(
            takeUntilDestroyed(this.destroyRef),
            map(currentUser => currentUser?.login === user.login),
        );
    }

    public onPrevious(): void {
        this.store.dispatch(usersPreviousPage());
    }

    public onNext(): void {
        this.store.dispatch(usersNextPage());
    }
}
