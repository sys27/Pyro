import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';
import { mapErrorToEmpty } from '../../../services/operators';
import { UserItem, UserService } from '../../../services/user.service';

@Component({
    selector: 'user-list',
    standalone: true,
    imports: [CommonModule, RouterModule, TableModule, ButtonModule],
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
})
export class UserListComponent implements OnInit {
    public users$: Observable<UserItem[]> | undefined;

    public constructor(private readonly userService: UserService) {}

    public ngOnInit(): void {
        this.users$ = this.userService.getUsers().pipe(mapErrorToEmpty);
    }
}
