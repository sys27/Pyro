import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { mapErrorToEmpty } from '@services/operators';
import { RepositoryItem, RepositoryService } from '@services/repository.service';
import { TableModule } from 'primeng/table';
import { Observable, shareReplay } from 'rxjs';

@Component({
    selector: 'repo-list',
    standalone: true,
    imports: [CommonModule, RouterModule, TableModule],
    templateUrl: './repository-list.component.html',
    styleUrl: './repository-list.component.css',
})
export class RepositoryListComponent implements OnInit {
    public repositories$: Observable<RepositoryItem[]> | undefined;

    public constructor(private readonly repoService: RepositoryService) {}

    public ngOnInit(): void {
        this.repositories$ = this.repoService
            .getRepositories()
            .pipe(mapErrorToEmpty, shareReplay(1));
    }
}
