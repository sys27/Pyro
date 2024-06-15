import { Component, OnInit, input } from '@angular/core';
import { User } from '../../models/user';
import { UserService } from '../../services/user.service';

@Component({
    selector: 'user',
    standalone: true,
    imports: [],
    templateUrl: './user.component.html',
    styleUrl: './user.component.css',
})
export class UserComponent implements OnInit {
    public email = input.required<string>();

    public user: User | undefined;

    public constructor(private readonly userService: UserService) {}

    public ngOnInit(): void {
        this.userService.getUser(this.email()).subscribe(user => (this.user = user));
    }
}
