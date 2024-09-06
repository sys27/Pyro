import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, DestroyRef, input, OnDestroy, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterModule } from '@angular/router';
import { PyroPermissions } from '@models/pyro-permissions';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { AuthService } from '@services/auth.service';
import { Comment, Issue, IssueService } from '@services/issue.service';
import { mapErrorToEmpty, mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { TagModule } from 'primeng/tag';
import { BehaviorSubject, combineLatest, map, Observable, shareReplay, switchMap } from 'rxjs';
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
        LuminanceColorPipe,
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
    public hasEditPermission$: Observable<boolean> | undefined;
    private readonly commentAddedTrigger$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly issueService: IssueService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.issue$ = this.issueService
            .getIssue(this.repositoryName(), this.issueNumber())
            .pipe(mapErrorToNull, shareReplay(1));
        this.comments$ = combineLatest([this.issue$!, this.commentAddedTrigger$]).pipe(
            switchMap(([issue, _]) => {
                if (!issue) {
                    return [];
                }

                return this.issueService.getIssueComments(this.repositoryName(), issue.issueNumber);
            }),
            mapErrorToEmpty,
            shareReplay(1),
        );
        this.hasEditPermission$ = this.authService.currentUser.pipe(
            takeUntilDestroyed(this.destroyRef),
            map(user => user?.hasPermission(PyroPermissions.IssueEdit) ?? false),
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
