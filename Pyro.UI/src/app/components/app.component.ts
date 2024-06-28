import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { MenuItem, PrimeNGConfig } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ButtonGroupModule } from 'primeng/buttongroup';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { SplitButtonModule } from 'primeng/splitbutton';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { AuthService } from '../services/auth.service';
import { ThemeService } from '../services/theme.service';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [
        CommonModule,
        ButtonModule,
        ButtonGroupModule,
        InputTextModule,
        IconFieldModule,
        InputIconModule,
        RouterOutlet,
        SplitButtonModule,
        RouterLink,
        RouterLinkActive,
        ToolbarModule,
        ToastModule,
    ],
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
    public title = 'Pyro';
    public themeIcon: string = 'pi pi-sun';
    public loginMenuItems: MenuItem[] = [
        { label: 'Settings', icon: 'pi pi-cog' },
        { label: 'Logout', icon: 'pi pi-sign-out' },
    ];

    public constructor(
        private readonly primeNg: PrimeNGConfig,
        private readonly router: Router,
        public readonly authService: AuthService,
        private readonly themeService: ThemeService,
    ) {}

    public ngOnInit(): void {
        this.themeService.useTheme();
        this.primeNg.ripple = true;
    }

    public toggleThemeClick(): void {
        let theme = this.themeService.toggleTheme();
        this.themeIcon = theme === 'dark' ? 'pi pi-sun' : 'pi pi-moon';
    }

    public profileOnClick(): void {
        this.router.navigate(['profile']);
    }
}
