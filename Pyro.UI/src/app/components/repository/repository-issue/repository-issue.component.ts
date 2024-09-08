import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, DestroyRef, Injector, input, OnDestroy, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterModule } from '@angular/router';
import { PyroPermissions } from '@models/pyro-permissions';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { AuthService } from '@services/auth.service';
import { Comment, Issue, IssueService } from '@services/issue.service';
import { createErrorHandler } from '@services/operators';
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
    public canEdit$: Observable<boolean> | undefined;
    public canManage$: Observable<boolean> | undefined;
    private readonly commentAddedTrigger$ = new BehaviorSubject<void>(undefined);
    private readonly refreshIssue$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly injector: Injector,
        private readonly destroyRef: DestroyRef,
        private readonly issueService: IssueService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.issue$ = this.refreshIssue$.pipe(
            switchMap(() => this.issueService.getIssue(this.repositoryName(), this.issueNumber())),
            createErrorHandler(this.injector),
            shareReplay(1),
        );
        this.comments$ = combineLatest([this.issue$!, this.commentAddedTrigger$]).pipe(
            switchMap(([issue, _]) => {
                if (!issue) {
                    return [];
                }

                return this.issueService.getIssueComments(this.repositoryName(), issue.issueNumber);
            }),
            createErrorHandler(this.injector),
            shareReplay(1),
        );
        this.canEdit$ = combineLatest([this.authService.currentUser, this.issue$]).pipe(
            takeUntilDestroyed(this.destroyRef),
            map(([user, issue]) => {
                if (!user || !issue) {
                    return false;
                }

                return user.hasPermission(PyroPermissions.IssueEdit) && !issue.isLocked;
            }),
        );
        this.canManage$ = this.authService.currentUser.pipe(
            takeUntilDestroyed(this.destroyRef),
            map(user => {
                if (!user) {
                    return false;
                }

                return user.hasPermission(PyroPermissions.IssueManage);
            }),
        );
    }

    public ngOnDestroy(): void {
        this.commentAddedTrigger$.complete();
        this.refreshIssue$.complete();
    }

    public commentAdded(): void {
        // TODO: do not refresh all comments, just add the new one
        this.commentAddedTrigger$.next();
    }

    public lockIssue(): void {
        this.issueService
            .lockIssue(this.repositoryName(), this.issueNumber())
            .subscribe(() => this.refreshIssue$.next());
    }

    public unlockIssue(): void {
        this.issueService
            .unlockIssue(this.repositoryName(), this.issueNumber())
            .subscribe(() => this.refreshIssue$.next());
    }
}
