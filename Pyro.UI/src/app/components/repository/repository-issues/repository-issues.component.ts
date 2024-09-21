import { issuesNextPage, issuesPreviousPage, loadIssues } from '@actions/issues.actions';
import { AsyncPipe, NgClass } from '@angular/common';
import { Component, input, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PaginatorComponent } from '@controls/paginator/paginator.component';
import { TagComponent } from '@controls/tag/tag.component';
import { DataSourceDirective } from '@directives/data-source.directive';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { Issue } from '@services/issue.service';
import { AppState } from '@states/app.state';
import { selectHasPermission } from '@states/auth.state';
import { DataSourceState } from '@states/data-source.state';
import { selectCurrentPage, selectHasNext, selectHasPrevious } from '@states/paged.state';
import { selectIssues } from '@states/repository.state';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { DividerModule } from 'primeng/divider';
import { Observable } from 'rxjs';

@Component({
    selector: 'repo-issues',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        ColorPipe,
        DataSourceDirective,
        DataViewModule,
        DividerModule,
        LuminanceColorPipe,
        NgClass,
        PaginatorComponent,
        RouterLink,
        TagComponent,
    ],
    templateUrl: './repository-issues.component.html',
    styleUrls: ['./repository-issues.component.css'],
})
export class RepositoryIssuesComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly issues$: Observable<DataSourceState<Issue>> = this.store.select(
        selectCurrentPage(selectIssues),
    );
    public readonly isPreviousEnabled$: Observable<boolean> = this.store.select(
        selectHasPrevious(selectIssues),
    );
    public readonly isNextEnabled$: Observable<boolean> = this.store.select(
        selectHasNext(selectIssues),
    );
    public hasEditPermission$: Observable<boolean> = this.store.select(
        selectHasPermission(PyroPermissions.IssueEdit),
    );

    public constructor(private readonly store: Store<AppState>) {}

    public ngOnInit(): void {
        this.store.dispatch(loadIssues({ repositoryName: this.repositoryName() }));
    }

    public onPrevious(): void {
        this.store.dispatch(issuesPreviousPage());
    }

    public onNext(): void {
        this.store.dispatch(issuesNextPage());
    }
}
