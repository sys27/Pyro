import { CommonModule } from '@angular/common';
import { Component, OnInit, input } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { TabMenuModule } from 'primeng/tabmenu';
import { Repository, RepositoryService } from '../../services/repository.service';

@Component({
    selector: 'repo-list',
    standalone: true,
    imports: [CommonModule, TabMenuModule],
    templateUrl: './repository.component.html',
    styleUrl: './repository.component.css',
})
export class RepositoryComponent implements OnInit {
    public readonly repositoryName = input.required<string>({ alias: 'name' });
    public readonly menu: MenuItem[] = [
        { label: 'Code', icon: 'pi pi-code', routerLink: 'code' },
        { label: 'Issues', icon: 'pi pi-ticket', routerLink: 'issues' },
        { label: 'Pull Requests', icon: 'pi pi-file-import', routerLink: 'pull-requests' },
        { label: 'Settings', icon: 'pi pi-cog', routerLink: 'settings' },
    ];

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
