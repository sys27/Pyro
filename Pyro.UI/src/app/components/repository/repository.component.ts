import { CommonModule } from '@angular/common';
import { Component, OnInit, input } from '@angular/core';
import { Repository, RepositoryService } from '../../services/repository.service';

@Component({
    selector: 'repo-list',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './repository.component.html',
    styleUrl: './repository.component.css',
})
export class RepositoryComponent implements OnInit {
    public readonly repositoryName = input.required<string>({ alias: 'name' });
    private _repository: Repository | undefined;

    public constructor(private repoService: RepositoryService) {}

    public ngOnInit(): void {
        this.repoService
            .getRepository(this.repositoryName())
            .subscribe(repo => (this._repository = repo));
    }

    public get repository(): Repository | undefined {
        return this._repository;
    }
}
