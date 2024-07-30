import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { Issue, IssueService } from '@services/issue.service';
import { mapErrorToEmpty } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { map, Observable, shareReplay, switchMap } from 'rxjs';

@Component({
    selector: 'repo-issues',
    standalone: true,
    imports: [ButtonModule, CommonModule, DataViewModule, RouterModule],
    templateUrl: './repository-issues.component.html',
    styleUrls: ['./repository-issues.component.css'],
})
export class RepositoryIssuesComponent implements OnInit {
    private repositoryName$: Observable<string> | undefined;
    public issues$: Observable<Issue[]> | undefined;

    public constructor(
        private readonly route: ActivatedRoute,
        private readonly issueService: IssueService,
    ) {}

    public ngOnInit(): void {
        this.repositoryName$ = this.route.parent?.params.pipe(map(params => params['name']));
        this.issues$ = this.repositoryName$?.pipe(
            switchMap(name => this.issueService.getIssues(name)),
            mapErrorToEmpty,
            shareReplay(1),
        );
    }
}
