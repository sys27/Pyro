import {
    forgotPassword,
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
import { DataSourceDirective } from '@directives/data-source.directive';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import { User, UserItem } from '@services/user.service';
import { AppState } from '@states/app.state';
import { selectCurrentUser, selectHasPermission } from '@states/auth.state';
import { DataSourceState } from '@states/data-source.state';
import { selectCurrentPage, selectHasNext, selectHasPrevious } from '@states/paged.state';
import { selectUsers } from '@states/users.state';
import { ButtonModule } from 'primeng/button';
import { ButtonGroupModule } from 'primeng/buttongroup';
import { TableModule } from 'primeng/table';
import { TooltipModule } from 'primeng/tooltip';
import { map, Observable } from 'rxjs';

@Component({
    selector: 'user-list',
    imports: [
        AsyncPipe,
        ButtonModule,
        ButtonGroupModule,
        DataSourceDirective,
        RouterModule,
        TableModule,
        TooltipModule,
    ],
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
})
export class UserListComponent implements OnInit {
    public readonly users$: Observable<DataSourceState<UserItem>> = this.store.select(
        selectCurrentPage(selectUsers),
    );
    public readonly isPreviousEnabled$: Observable<boolean> = this.store.select(
        selectHasPrevious(selectUsers),
    );
    public readonly isNextEnabled$: Observable<boolean> = this.store.select(
        selectHasNext(selectUsers),
    );
    public readonly hasEditPermission$: Observable<boolean> = this.store.select(
        selectHasPermission(PyroPermissions.UserEdit),
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

    public resetPasswordHandler(user: User): void {
        this.store.dispatch(forgotPassword({ login: user.login }));
    }

    public isCurrentUser(user: User): Observable<boolean> {
        return this.store.select(selectCurrentUser).pipe(
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
