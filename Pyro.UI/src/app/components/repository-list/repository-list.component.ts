import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { Observable } from 'rxjs';
import { mapErrorToEmpty } from '../../services/operators';
import { RepositoryItem, RepositoryService } from '../../services/repository.service';

@Component({
    selector: 'repo-list',
    standalone: true,
    imports: [CommonModule, RouterModule],
    templateUrl: './repository-list.component.html',
    styleUrl: './repository-list.component.css',
})
export class RepositoryListComponent implements OnInit {
    public repositories: Observable<RepositoryItem[]> | undefined;

    public constructor(private readonly repoService: RepositoryService) {}

    public ngOnInit(): void {
        this.repositories = this.repoService.getRepositories().pipe(mapErrorToEmpty);
    }
}
