import { AsyncPipe, NgClass } from '@angular/common';
import { Component, DestroyRef, input, OnInit, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PaginatorComponent, PaginatorState } from '@controls/paginator/paginator.component';
import { PyroPermissions } from '@models/pyro-permissions';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { AuthService } from '@services/auth.service';
import { Issue, IssueService } from '@services/issue.service';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { DividerModule } from 'primeng/divider';
import { TagModule } from 'primeng/tag';
import { map, Observable, of } from 'rxjs';

@Component({
    selector: 'repo-issues',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        ColorPipe,
        DataViewModule,
        DividerModule,
        LuminanceColorPipe,
        NgClass,
        PaginatorComponent,
        RouterLink,
        TagModule,
    ],
    templateUrl: './repository-issues.component.html',
    styleUrls: ['./repository-issues.component.css'],
})
export class RepositoryIssuesComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly issues = signal<Issue[]>([]);
    public hasEditPermission$: Observable<boolean> | undefined;

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly issueService: IssueService,
        private readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.hasEditPermission$ = this.authService.currentUser.pipe(
            map(user => user?.hasPermission(PyroPermissions.IssueEdit) ?? false),
        );
    }

    public paginatorLoader = (state: PaginatorState): Observable<Issue[]> => {
        if (!this.repositoryName()) {
            return of([]);
        }

        return this.issueService.getIssues(this.repositoryName(), state.before, state.after);
    };

    public paginatorOffsetSelector(item: Issue): string {
        return item.id;
    }

    public paginatorDataChanged(items: Issue[]): void {
        this.issues.set(items);
    }
}
