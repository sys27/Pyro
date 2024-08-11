import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { PaginatorComponent, PaginatorState } from '@controls/paginator/paginator.component';
import { Issue, IssueService } from '@services/issue.service';
import { mapErrorToEmpty } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { map, Observable, of, switchMap } from 'rxjs';

@Component({
    selector: 'repo-issues',
    standalone: true,
    imports: [ButtonModule, CommonModule, DataViewModule, PaginatorComponent, RouterModule],
    templateUrl: './repository-issues.component.html',
    styleUrls: ['./repository-issues.component.css'],
})
export class RepositoryIssuesComponent implements OnInit {
    private repositoryName$: Observable<string> | undefined;
    public issues: Issue[] = [];

    public constructor(
        private readonly route: ActivatedRoute,
        private readonly issueService: IssueService,
    ) {}

    public ngOnInit(): void {
        this.repositoryName$ = this.route.parent?.params.pipe(map(params => params['name']));
    }

    public paginatorLoader = (state: PaginatorState): Observable<Issue[]> => {
        if (!this.repositoryName$) {
            return of([]);
        }

        return this.repositoryName$.pipe(
            switchMap(name => this.issueService.getIssues(name, state.before, state.after)),
            mapErrorToEmpty,
        );
    };

    public paginatorOffsetSelector(item: Issue): string {
        return item.id;
    }

    public paginatorDataChanged(items: Issue[]): void {
        this.issues = items;
    }
}
