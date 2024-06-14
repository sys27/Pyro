import { Component, OnInit, input } from '@angular/core';
import { Observable } from 'rxjs';
import { Repository } from '../../models/repository';
import { RepositoryService } from '../../services/repository.service';
import { CommonModule } from '@angular/common';

@Component({
    selector: 'repo-list',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './repository.component.html',
    styleUrl: './repository.component.css',
})
export class RepositoryComponent implements OnInit {
    public readonly repositoryName = input.required<string>({ alias: 'name' });
    public repository: Observable<Repository> | undefined;

    public constructor(private repoService: RepositoryService) {}

    public ngOnInit(): void {
        this.repository = this.repoService.getRepository(this.repositoryName());
    }
}
