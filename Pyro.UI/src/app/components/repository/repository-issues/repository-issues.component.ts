import { NgClass } from '@angular/common';
import { Component, input, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { PaginatorComponent, PaginatorState } from '@controls/paginator/paginator.component';
import { ColorPipe } from '@pipes/color.pipe';
import { Issue, IssueService } from '@services/issue.service';
import { mapErrorToEmpty } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { TagModule } from 'primeng/tag';
import { Observable, of } from 'rxjs';

@Component({
    selector: 'repo-issues',
    standalone: true,
    imports: [
        ButtonModule,
        ColorPipe,
        DataViewModule,
        NgClass,
        PaginatorComponent,
        RouterLink,
        TagModule,
    ],
    templateUrl: './repository-issues.component.html',
    styleUrls: ['./repository-issues.component.css'],
})
export class RepositoryIssuesComponent {
    public readonly repositoryName = input.required<string>();
    public readonly issues = signal<Issue[]>([]);

    public constructor(private readonly issueService: IssueService) {}

    public paginatorLoader = (state: PaginatorState): Observable<Issue[]> => {
        if (!this.repositoryName()) {
            return of([]);
        }

        return this.issueService
            .getIssues(this.repositoryName(), state.before, state.after)
            .pipe(mapErrorToEmpty);
    };

    public paginatorOffsetSelector(item: Issue): string {
        return item.id;
    }

    public paginatorDataChanged(items: Issue[]): void {
        this.issues.set(items);
    }
}
