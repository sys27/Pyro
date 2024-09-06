import { Component, DestroyRef, OnInit, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterModule } from '@angular/router';
import { PaginatorComponent, PaginatorState } from '@controls/paginator/paginator.component';
import { PyroPermissions } from '@models/pyro-permissions';
import { AuthService } from '@services/auth.service';
import { mapErrorToEmpty } from '@services/operators';
import { UserItem, UserService } from '@services/user.service';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { map, Observable } from 'rxjs';

@Component({
    selector: 'user-list',
    standalone: true,
    imports: [ButtonModule, PaginatorComponent, RouterModule, TableModule],
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
})
export class UserListComponent implements OnInit {
    public readonly users = signal<UserItem[]>([]);
    public hasEditPermission$: Observable<boolean> | undefined;

    public constructor(
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

    public paginatorLoader = (state: PaginatorState): Observable<UserItem[]> => {
        return this.userService.getUsers(state.before, state.after).pipe(mapErrorToEmpty);
    };

    public paginatorOffsetSelector(item: UserItem): string {
        return item.login;
    }

    public paginatorDataChanged(items: UserItem[]): void {
        this.users.set(items);
    }
}
