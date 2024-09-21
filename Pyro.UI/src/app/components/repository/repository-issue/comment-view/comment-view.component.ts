import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, input, OnInit, output } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { Store } from '@ngrx/store';
import { Comment, Issue } from '@services/issue.service';
import { AppState } from '@states/app.state';
import { selectCurrentUser } from '@states/auth.state';
import { PanelModule } from 'primeng/panel';
import { combineLatest, map, Observable } from 'rxjs';
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
    public readonly issue = input.required<Issue>();
    private readonly issue$ = toObservable(this.issue);
    public isEditMode: boolean = false;
    public canEditIssue$: Observable<boolean> | undefined;
    public readonly commentAdded = output<Comment>();

    public constructor(private readonly store: Store<AppState>) {}

    public ngOnInit(): void {
        let currentUser$ = this.store.select(selectCurrentUser);
        this.canEditIssue$ = combineLatest([currentUser$, this.issue$]).pipe(
            map(([user, issue]) => {
                if (!user || !issue) {
                    return false;
                }

                return this.comment().author.id === user.id && !issue.isLocked;
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
