import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { PanelMenuModule } from 'primeng/panelmenu';

@Component({
    selector: 'settings',
    standalone: true,
    imports: [PanelMenuModule],
    templateUrl: './settings.component.html',
    styleUrl: './settings.component.css',
})
export class SettingsComponent implements OnInit {
    public menuItems: MenuItem[] | undefined;

    public ngOnInit(): void {
        this.menuItems = [
            { label: 'Profile', icon: 'pi pi-cog', routerLink: ['profile'] },
            { label: 'Access Tokens', icon: 'pi pi-key', routerLink: ['access-tokens'] },
        ];
    }
}
