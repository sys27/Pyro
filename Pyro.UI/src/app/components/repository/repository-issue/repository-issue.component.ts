import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, input, OnDestroy, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { ColorPipe } from '@pipes/color.pipe';
import { AuthService } from '@services/auth.service';
import { Comment, Issue, IssueService } from '@services/issue.service';
import { mapErrorToEmpty, mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { TagModule } from 'primeng/tag';
import {
    BehaviorSubject,
    combineLatest,
    map,
    Observable,
    shareReplay,
    switchMap,
    withLatestFrom,
} from 'rxjs';
import { MarkdownPipe } from '../../../pipes/markdown.pipe';
import { CommentNewComponent } from './comment-new/comment-new.component';
import { CommentViewComponent } from './comment-view/comment-view.component';

@Component({
    selector: 'repo-issue',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        ColorPipe,
        CommentNewComponent,
        CommentViewComponent,
        DatePipe,
        DividerModule,
        MarkdownPipe,
        RouterModule,
        TagModule,
    ],
    templateUrl: './repository-issue.component.html',
    styleUrl: './repository-issue.component.css',
})
export class RepositoryIssueComponent implements OnInit, OnDestroy {
    public readonly repositoryName = input.required<string>();
    public readonly issueNumber = input.required<number>();
    public issue$: Observable<Issue | null> | undefined;
    public comments$: Observable<Comment[]> | undefined;
    public canEditIssue$: Observable<boolean> | undefined;
    private readonly commentAddedTrigger$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly issueService: IssueService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.issue$ = this.issueService
            .getIssue(this.repositoryName(), this.issueNumber())
            .pipe(mapErrorToNull, shareReplay(1));
        this.comments$ = combineLatest([this.issue$!, this.commentAddedTrigger$]).pipe(
            switchMap(([issue, name]) => {
                if (!issue) {
                    return [];
                }

                return this.issueService.getIssueComments(this.repositoryName(), issue.issueNumber);
            }),
            mapErrorToEmpty,
            shareReplay(1),
        );
        this.canEditIssue$ = this.issue$?.pipe(
            withLatestFrom(this.authService.currentUser),
            map(([issue, user]) => {
                if (!issue || !user) {
                    return false;
                }

                return issue.author.id === user.id;
            }),
        );
    }

    public ngOnDestroy(): void {
        this.commentAddedTrigger$.complete();
    }

    public commentAdded(): void {
        // TODO: do not refresh all comments, just add the new one
        this.commentAddedTrigger$.next();
    }
}
