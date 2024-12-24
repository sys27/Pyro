import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MenuItem } from 'primeng/api';
import { PanelMenuModule } from 'primeng/panelmenu';

@Component({
    selector: 'repo-settings',
    imports: [PanelMenuModule, RouterOutlet],
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
