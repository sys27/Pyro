import { DOCUMENT } from '@angular/common';
import { Inject, Injectable } from '@angular/core';

@Injectable({
    providedIn: 'root',
})
export class ThemeService {
    public constructor(@Inject(DOCUMENT) private readonly document: Document) {}

    private getTheme(): Theme {
        let theme = localStorage.getItem('theme');
        if (theme === 'light' || theme === 'dark') {
            return theme;
        }

        return 'dark';
    }

    private setTheme(theme: Theme): void {
        localStorage.setItem('theme', theme);
    }

    public useTheme(): void {
        let theme = this.getTheme();
        let themeLink = this.document.getElementById('app-theme') as HTMLLinkElement;
        if (themeLink) {
            themeLink.href = `${theme}.css`;
        }

        let element = document.querySelector('html');
        if (element) {
            if (theme === 'dark') {
                element.classList.add('my-app-dark');
            } else {
                element.classList.remove('my-app-dark');
            }
        }
    }

    public toggleTheme(): Theme {
        let theme = this.getTheme();
        theme = theme === 'dark' ? 'light' : 'dark';
        this.setTheme(theme);
        this.useTheme();

        return theme;
    }
}

export type Theme = 'light' | 'dark';
