import { Component, OnInit } from '@angular/core';
import { RepositoryService } from '../../services/repository.service';
import { ButtonModule } from 'primeng/button';
import { Observable } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Repository } from '../../models/Repository';

@Component({
    selector: 'app-repo-list',
    standalone: true,
    imports: [CommonModule, ButtonModule],
    templateUrl: './repository-list.component.html',
    styleUrl: './repository-list.component.css'
})
export class RepositoryListComponent implements OnInit {
    public repositories$: Observable<Repository[]> | undefined;

    public constructor(
        private repoService: RepositoryService
    ) { }

    public ngOnInit(): void {
        this.repositories$ = this.repoService.getRepositories();
    }
}
