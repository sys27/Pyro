import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { Observable } from 'rxjs';
import { Repository } from '../../models/repository';
import { RepositoryService } from '../../services/repository.service';

@Component({
    selector: 'app-repo-list',
    standalone: true,
    imports: [CommonModule, ButtonModule],
    templateUrl: './repository-list.component.html',
    styleUrl: './repository-list.component.css',
})
export class RepositoryListComponent implements OnInit {
    public repositories$: Observable<Repository[]> | undefined;

    public constructor(private repoService: RepositoryService) {}

    public ngOnInit(): void {
        this.repositories$ = this.repoService.getRepositories();
    }
}
