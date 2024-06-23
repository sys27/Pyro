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
import { ToolbarModule } from 'primeng/toolbar';
import { AuthService } from '../services/auth.service';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [
        CommonModule,
        RouterOutlet,
        RouterLink,
        RouterLinkActive,
        ToolbarModule,
        ButtonModule,
        ButtonGroupModule,
        InputTextModule,
        IconFieldModule,
        InputIconModule,
        SplitButtonModule,
    ],
    templateUrl: './app.component.html',
    styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
    public title = 'Pyro';
    public loginMenuItems: MenuItem[] = [
        { label: 'Settings', icon: 'pi pi-cog' },
        { label: 'Logout', icon: 'pi pi-sign-out' },
    ];

    public constructor(
        private readonly primeNg: PrimeNGConfig,
        private readonly router: Router,
        public readonly authService: AuthService,
    ) {}

    public ngOnInit(): void {
        this.primeNg.ripple = true;
    }

    public profileOnClick(): void {
        this.router.navigate(['profile']);
    }
}
