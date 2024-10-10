import { AsyncPipe, NgClass } from '@angular/common';
import { Component, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PaginatorComponent, PaginatorState } from '@controls/paginator/paginator.component';
import { TagComponent } from '@controls/tag/tag.component';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { Issue, IssueService } from '@services/issue.service';
import { AppState } from '@states/app.state';
import { hasPermissionSelector } from '@states/auth.state';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { DividerModule } from 'primeng/divider';
import { Observable, of } from 'rxjs';

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
        TagComponent,
    ],
    templateUrl: './repository-issues.component.html',
    styleUrls: ['./repository-issues.component.css'],
})
export class RepositoryIssuesComponent {
    public readonly repositoryName = input.required<string>();
    public readonly issues = signal<Issue[]>([]);
    public hasEditPermission$: Observable<boolean> = this.store.select(
        hasPermissionSelector(PyroPermissions.IssueEdit),
    );

    public constructor(
        private readonly issueService: IssueService,
        private readonly store: Store<AppState>,
    ) {}

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
