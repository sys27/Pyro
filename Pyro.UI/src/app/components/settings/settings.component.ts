import { Component } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { PanelMenuModule } from 'primeng/panelmenu';

@Component({
    selector: 'settings',
    standalone: true,
    imports: [PanelMenuModule],
    templateUrl: './settings.component.html',
    styleUrl: './settings.component.css',
})
export class SettingsComponent {
    public readonly menuItems: MenuItem[] = [
        { label: 'Profile', icon: 'pi pi-cog', routerLink: ['profile'] },
        { label: 'Access Tokens', icon: 'pi pi-key', routerLink: ['access-tokens'] },
        { label: 'Change Password', icon: 'pi pi-key', routerLink: ['change-password'] },
    ];
}
