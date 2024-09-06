import { Component, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { PaginatorComponent, PaginatorState } from '@controls/paginator/paginator.component';
import { RepositoryItem, RepositoryService } from '@services/repository.service';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';

@Component({
    selector: 'repo-list',
    standalone: true,
    imports: [PaginatorComponent, RouterModule, TableModule],
    templateUrl: './repository-list.component.html',
    styleUrl: './repository-list.component.css',
})
export class RepositoryListComponent {
    public repositories = signal<RepositoryItem[]>([]);

    public constructor(private readonly repoService: RepositoryService) {}

    public paginatorLoader = (state: PaginatorState): Observable<RepositoryItem[]> => {
        return this.repoService.getRepositories(state.before, state.after);
    };

    public paginatorOffsetSelector(item: RepositoryItem): string {
        return item.name;
    }

    public paginatorDataChanged(items: RepositoryItem[]): void {
        this.repositories.set(items);
    }
}
