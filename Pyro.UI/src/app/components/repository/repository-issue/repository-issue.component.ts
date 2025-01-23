import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, DestroyRef, Injector, input, OnDestroy, OnInit } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterModule } from '@angular/router';
import { TagComponent } from '@controls/tag/tag.component';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import {
    ChangeLogs,
    Comment,
    Issue,
    IssueChangeLogType,
    IssueService,
} from '@services/issue.service';
import { createErrorHandler } from '@services/operators';
import { AppState } from '@states/app.state';
import { selectCurrentUser, selectHasPermission } from '@states/auth.state';
import { ButtonModule } from 'primeng/button';
import { ButtonGroupModule } from 'primeng/buttongroup';
import { DividerModule } from 'primeng/divider';
import { TooltipModule } from 'primeng/tooltip';
import { BehaviorSubject, combineLatest, map, Observable, shareReplay, switchMap } from 'rxjs';
import { ChangeLogViewComponent } from './change-log-view/change-log-view.component';
import { CommentNewComponent } from './comment-new/comment-new.component';
import { CommentViewComponent } from './comment-view/comment-view.component';

@Component({
    selector: 'repo-issue',
    imports: [
        AsyncPipe,
        ButtonModule,
        ButtonGroupModule,
        ChangeLogViewComponent,
        CommentNewComponent,
        CommentViewComponent,
        DatePipe,
        DividerModule,
        RouterModule,
        TagComponent,
        TooltipModule,
    ],
    templateUrl: './repository-issue.component.html',
    styleUrl: './repository-issue.component.css',
})
export class RepositoryIssueComponent implements OnInit, OnDestroy {
    public readonly repositoryName = input.required<string>();
    public readonly issueNumber = input.required<number>();
    public issue$: Observable<Issue | null> | undefined;
    public comments$: Observable<Comment[]> | undefined;
    public changeLogs$: Observable<ChangeLogs[]> | undefined;
    public items$: Observable<(Comment | ChangeLogs)[]> | undefined;
    public canEdit$: Observable<boolean> | undefined;
    public canManage$: Observable<boolean> = this.store.select(
        selectHasPermission(PyroPermissions.IssueManage),
    );
    private readonly commentAddedTrigger$ = new BehaviorSubject<void>(undefined);
    private readonly refreshIssue$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly injector: Injector,
        private readonly destroyRef: DestroyRef,
        private readonly issueService: IssueService,
        private readonly store: Store<AppState>,
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
        this.changeLogs$ = this.issue$.pipe(
            switchMap(issue => {
                if (!issue) {
                    return [];
                }

                return this.issueService.getIssueChangeLogs(
                    this.repositoryName(),
                    issue.issueNumber,
                );
            }),
            createErrorHandler(this.injector),
            shareReplay(1),
        );
        this.items$ = combineLatest([this.comments$, this.changeLogs$]).pipe(
            map(([comments, changeLogs]) => {
                return [...comments, ...changeLogs].sort(
                    (a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime(),
                );
            }),
        );
        this.canEdit$ = combineLatest([this.store.select(selectCurrentUser), this.issue$]).pipe(
            takeUntilDestroyed(this.destroyRef),
            map(([user, issue]) => {
                if (!user || !issue) {
                    return false;
                }

                return user.hasPermission(PyroPermissions.IssueEdit) && !issue.isLocked;
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
            .pipe(createErrorHandler(this.injector))
            .subscribe(() => this.refreshIssue$.next());
    }

    public unlockIssue(): void {
        this.issueService
            .unlockIssue(this.repositoryName(), this.issueNumber())
            .pipe(createErrorHandler(this.injector))
            .subscribe(() => this.refreshIssue$.next());
    }

    public isChangeLog(obj: Comment | ChangeLogs): obj is ChangeLogs {
        return (
            obj &&
            '$type' in obj &&
            typeof obj.$type === 'number' &&
            Object.values(IssueChangeLogType).includes(obj.$type)
        );
    }
}
