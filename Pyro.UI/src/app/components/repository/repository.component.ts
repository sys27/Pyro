import { CommonModule } from '@angular/common';
import { Component, input } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { TabMenuModule } from 'primeng/tabmenu';

@Component({
    selector: 'repo-list',
    standalone: true,
    imports: [CommonModule, TabMenuModule],
    templateUrl: './repository.component.html',
    styleUrl: './repository.component.css',
})
export class RepositoryComponent {
    public readonly repositoryName = input.required<string>({ alias: 'name' });

    public menu: MenuItem[] = [
        { label: 'Code', icon: 'pi pi-code', routerLink: 'code' },
        { label: 'Issues', icon: 'pi pi-ticket', routerLink: 'issues' },
        { label: 'Pull Requests', icon: 'pi pi-file-import', routerLink: 'pull-requests' },
        { label: 'Settings', icon: 'pi pi-cog', routerLink: 'settings' },
    ];
}
