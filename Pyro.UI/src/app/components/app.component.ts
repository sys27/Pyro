import { logoutAction } from '@actions/auth.actions';
import { AsyncPipe } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';
import { ThemeService } from '@services/theme.service';
import { AppState } from '@states/app.state';
import { selectIsLoggedIn } from '@states/auth.state';
import { MenuItem, PrimeNGConfig } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { ButtonGroupModule } from 'primeng/buttongroup';
import { IconFieldModule } from 'primeng/iconfield';
import { InputIconModule } from 'primeng/inputicon';
import { InputTextModule } from 'primeng/inputtext';
import { SplitButtonModule } from 'primeng/splitbutton';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { Observable } from 'rxjs';

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
export class AppComponent implements OnInit {
    public title = 'Pyro';
    public themeIcon = signal('pi pi-sun');
    public loginMenuItems: MenuItem[] = [
        { label: 'Settings', icon: 'pi pi-cog' },
        {
            label: 'Logout',
            icon: 'pi pi-sign-out',
            command: () => {
                this.store.dispatch(logoutAction());
            },
        },
    ];
    public isLoggedIn$: Observable<boolean> = this.store.select(selectIsLoggedIn);

    public constructor(
        private readonly primeNg: PrimeNGConfig,
        private readonly router: Router,
        private readonly store: Store<AppState>,
        private readonly themeService: ThemeService,
    ) {}

    public ngOnInit(): void {
        this.themeService.useTheme();
        this.primeNg.ripple = true;
    }

    public toggleThemeClick(): void {
        let theme = this.themeService.toggleTheme();
        this.themeIcon.set(theme === 'dark' ? 'pi pi-sun' : 'pi pi-moon');
    }

    public profileOnClick(): void {
        this.router.navigate(['settings']);
    }
}
