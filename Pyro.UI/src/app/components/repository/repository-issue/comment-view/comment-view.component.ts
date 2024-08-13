import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, input, OnInit, output } from '@angular/core';
import { AuthService } from '@services/auth.service';
import { Comment } from '@services/issue.service';
import { PanelModule } from 'primeng/panel';
import { map, Observable } from 'rxjs';
import { MarkdownPipe } from '../../../../pipes/markdown.pipe';
import { CommentNewComponent } from '../comment-new/comment-new.component';

@Component({
    selector: 'comment-view',
    standalone: true,
    imports: [AsyncPipe, CommentNewComponent, DatePipe, MarkdownPipe, PanelModule],
    templateUrl: './comment-view.component.html',
    styleUrl: './comment-view.component.css',
})
export class CommentViewComponent implements OnInit {
    public readonly comment = input.required<Comment>();
    public readonly repositoryName = input.required<string>();
    public readonly issueNumber = input.required<number>();
    public isEditMode: boolean = false;
    public canEditIssue$: Observable<boolean> | undefined;
    public readonly commentAdded = output<Comment>();

    public constructor(private readonly authService: AuthService) {}

    public ngOnInit(): void {
        this.canEditIssue$ = this.authService.currentUser.pipe(
            map(user => {
                if (!user) {
                    return false;
                }

                return this.comment().author.id === user.id;
            }),
        );
    }

    public toggleEditMode(): void {
        this.isEditMode = true;
    }

    public onCommentAdded(comment: Comment): void {
        this.isEditMode = false;
        this.commentAdded.emit(comment);
    }

    public onCancel(): void {
        this.isEditMode = false;
    }
}
