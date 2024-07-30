import { CommonModule } from '@angular/common';
import { Component, input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { AuthService } from '@services/auth.service';
import { Comment, Issue, IssueService } from '@services/issue.service';
import { mapErrorToEmpty, mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import {
    BehaviorSubject,
    combineLatest,
    map,
    Observable,
    shareReplay,
    switchMap,
    withLatestFrom,
} from 'rxjs';
import { MarkdownPipe } from '../../../markdown.pipe';
import { CommentNewComponent } from './comment-new/comment-new.component';
import { CommentViewComponent } from './comment-view/comment-view.component';

@Component({
    selector: 'repo-issue',
    standalone: true,
    imports: [
        ButtonModule,
        CommentNewComponent,
        CommentViewComponent,
        CommonModule,
        DividerModule,
        MarkdownPipe,
        RouterModule,
    ],
    templateUrl: './repository-issue.component.html',
    styleUrl: './repository-issue.component.css',
})
export class RepositoryIssueComponent implements OnInit, OnDestroy {
    public repositoryName$: Observable<string> | undefined;
    public readonly issueNumber = input.required<number>();
    public issue$: Observable<Issue | null> | undefined;
    public comments$: Observable<Comment[]> | undefined;
    public canEditIssue$: Observable<boolean> | undefined;
    private readonly commentAddedTrigger$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly route: ActivatedRoute,
        private readonly issueService: IssueService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.repositoryName$ = this.route.parent?.params.pipe(map(params => params['name']));
        this.issue$ = this.repositoryName$?.pipe(
            switchMap(name => this.issueService.getIssue(name, this.issueNumber())),
            mapErrorToNull,
            shareReplay(1),
        );
        this.comments$ = combineLatest([this.issue$!, this.commentAddedTrigger$]).pipe(
            withLatestFrom(this.repositoryName$!),
            switchMap(([[issue, _], name]) => {
                if (!issue) {
                    return [];
                }

                return this.issueService.getIssueComments(name, issue.issueNumber);
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
