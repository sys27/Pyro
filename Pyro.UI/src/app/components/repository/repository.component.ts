import { CommonModule } from '@angular/common';
import { Component, OnInit, input } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { TabMenuModule } from 'primeng/tabmenu';
import { RepositoryService } from '../../services/repository.service';

@Component({
    selector: 'repo-list',
    standalone: true,
    imports: [CommonModule, TabMenuModule],
    templateUrl: './repository.component.html',
    styleUrl: './repository.component.css',
})
export class RepositoryComponent implements OnInit {
    public readonly repositoryName = input.required<string>({ alias: 'name' });

    public menu: MenuItem[] | undefined;

    public constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly repoService: RepositoryService,
    ) {}

    public ngOnInit(): void {
        this.repoService.getRepository(this.repositoryName()).subscribe(repo => {
            this.menu = [
                { label: 'Code', icon: 'pi pi-code', routerLink: ['code', repo.defaultBranch] },
                { label: 'Issues', icon: 'pi pi-ticket', routerLink: 'issues' },
                { label: 'Pull Requests', icon: 'pi pi-file-import', routerLink: 'pull-requests' },
                { label: 'Settings', icon: 'pi pi-cog', routerLink: 'settings' },
            ];

            this.router.navigate(['code', repo.defaultBranch], { relativeTo: this.route });
        });
    }
}
