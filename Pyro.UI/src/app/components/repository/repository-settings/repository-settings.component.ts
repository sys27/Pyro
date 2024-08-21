import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { PanelMenuModule } from 'primeng/panelmenu';

@Component({
    selector: 'repo-settings',
    standalone: true,
    imports: [PanelMenuModule],
    templateUrl: './repository-settings.component.html',
    styleUrl: './repository-settings.component.css',
})
export class RepositorySettingsComponent {
    public readonly menuItems: MenuItem[] = [
        { label: 'Labels', icon: 'pi pi-tags', routerLink: ['labels'] },
        { label: 'Statuses', icon: 'pi pi-bars', routerLink: ['statuses'] },
        {
            label: 'Status Transitions',
            icon: 'pi pi-bars',
            routerLink: ['statuses', 'transitions'],
        },
    ];
}
