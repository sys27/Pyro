import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PaginatorComponent, PaginatorState } from '@controls/paginator/paginator.component';
import { mapErrorToEmpty } from '@services/operators';
import { UserItem, UserService } from '@services/user.service';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';

@Component({
    selector: 'user-list',
    standalone: true,
    imports: [CommonModule, PaginatorComponent, RouterModule, TableModule, ButtonModule],
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
})
export class UserListComponent {
    public users: UserItem[] = [];

    public constructor(private readonly userService: UserService) {}

    public paginatorLoader = (state: PaginatorState): Observable<UserItem[]> => {
        return this.userService.getUsers(state.before, state.after).pipe(mapErrorToEmpty);
    };

    public paginatorOffsetSelector(item: UserItem): string {
        return item.login;
    }

    public paginatorDataChanged(items: UserItem[]): void {
        this.users = items;
    }
}
