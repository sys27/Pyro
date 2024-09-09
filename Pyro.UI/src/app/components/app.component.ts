import { AsyncPipe } from '@angular/common';
import { Component, Injector, OnDestroy, OnInit, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '@services/auth.service';
import { NotificationService } from '@services/notification.service';
import { createErrorHandler } from '@services/operators';
import { ThemeService } from '@services/theme.service';
import { MenuItem, PrimeNGConfig } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ButtonGroupModule } from 'primeng/buttongroup';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { SplitButtonModule } from 'primeng/splitbutton';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { finalize, noop } from 'rxjs';

@Component({
    selector: 'app-root',
    standalone: true,
    imports: [
        AsyncPipe,
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
export class AppComponent implements OnInit, OnDestroy {
    public title = 'Pyro';
    public themeIcon = signal('pi pi-sun');
    public loginMenuItems: MenuItem[] = [
        { label: 'Settings', icon: 'pi pi-cog' },
        {
            label: 'Logout',
            icon: 'pi pi-sign-out',
            command: () => {
                this.authService
                    .logout()
                    .pipe(
                        createErrorHandler(this.injector),
                        finalize(() => this.router.navigate(['login'])),
                    )
                    .subscribe(noop);
            },
        },
    ];

    public constructor(
        private readonly injector: Injector,
        private readonly primeNg: PrimeNGConfig,
        private readonly router: Router,
        public readonly authService: AuthService,
        private readonly themeService: ThemeService,
        private readonly notificationService: NotificationService,
    ) {}

    public ngOnInit(): void {
        this.themeService.useTheme();
        this.primeNg.ripple = true;
    }

    public ngOnDestroy(): void {
        this.notificationService.ngOnDestroy();
    }

    public toggleThemeClick(): void {
        let theme = this.themeService.toggleTheme();
        this.themeIcon.set(theme === 'dark' ? 'pi pi-sun' : 'pi pi-moon');
    }

    public profileOnClick(): void {
        this.router.navigate(['settings']);
    }
}
