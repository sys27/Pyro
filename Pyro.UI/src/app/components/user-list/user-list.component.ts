import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { UserItem, UserService } from '../../services/user.service';

@Component({
    selector: 'user-list',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './user-list.component.html',
    styleUrl: './user-list.component.css',
})
export class UserListComponent implements OnInit {
    public users: Observable<UserItem[]> | undefined;

    public constructor(private readonly userService: UserService) {}

    public ngOnInit(): void {
        this.users = this.userService.getUsers();
    }
}
