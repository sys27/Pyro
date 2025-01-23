import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { ApplicationConfig, isDevMode, provideZoneChangeDetection } from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter, withComponentInputBinding, withRouterConfig } from '@angular/router';
import * as errorsEffects from '@effects/errors.effects';
import { provideEffects } from '@ngrx/effects';
import { provideRouterStore, routerReducer } from '@ngrx/router-store';
import { provideStore } from '@ngrx/store';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { definePreset } from '@primeng/themes';
import Aura from '@primeng/themes/aura';
import { profileReducer } from '@reducers/profile.reducers';
import { repositoriesReducer } from '@reducers/repositories.reducers';
import { repositoryReducer } from '@reducers/repository.reducers';
import { rolesReducer } from '@reducers/roles.reducers';
import { usersReducer } from '@reducers/users.reducers';
import { webSocketReducer } from '@reducers/web-socket.reducers';
import { authInterceptor } from '@services/auth.interceptor';
import { AppState } from '@states/app.state';
import { MessageService } from 'primeng/api';
import { providePrimeNG } from 'primeng/config';
import { routes } from './app.routes';
import * as accessTokensEffects from './effects/access-tokens.effects';
import * as authEffects from './effects/auth.effects';
import * as issuesEffects from './effects/issues.effects';
import * as notificationEffects from './effects/notification.effects';
import * as profileEffects from './effects/profile.effects';
import * as repositoriesEffects from './effects/repositories.effects';
import * as repositoryLabelsEffects from './effects/repository-labels.effects';
import * as repositoryStatusesEffects from './effects/repository-statuses.effects';
import * as repositoryEffects from './effects/repository.effects';
import * as rolessEffects from './effects/roles.effects';
import * as usersEffects from './effects/users.effects';
import * as webSocketEffects from './effects/web-socket.effects';
import { authReducer, saveStateReducer } from './reducers/auth.reducers';

export const appConfig: ApplicationConfig = {
    providers: [
        MessageService,
        provideZoneChangeDetection({ eventCoalescing: true }),
        provideRouter(
            routes,
            withComponentInputBinding(),
            withRouterConfig({ paramsInheritanceStrategy: 'always' }),
        ),
        provideHttpClient(withInterceptors([authInterceptor])),
        provideAnimationsAsync(),
        providePrimeNG({
            ripple: true,
            theme: {
                preset: definePreset(Aura, {
                    semantic: {
                        // primary: {
                        //     50: '{blue.50}',
                        //     100: '{blue.100}',
                        //     200: '{blue.200}',
                        //     300: '{blue.300}',
                        //     400: '{blue.400}',
                        //     500: '{blue.500}',
                        //     600: '{blue.600}',
                        //     700: '{blue.700}',
                        //     800: '{blue.800}',
                        //     900: '{blue.900}',
                        //     950: '{blue.950}',
                        // },
                        // colorScheme: {
                        //     light: {
                        //         surface: {
                        //             0: '#ffffff',
                        //             50: '{slate.50}',
                        //             100: '{slate.100}',
                        //             200: '{slate.200}',
                        //             300: '{slate.300}',
                        //             400: '{slate.400}',
                        //             500: '{slate.500}',
                        //             600: '{slate.600}',
                        //             700: '{slate.700}',
                        //             800: '{slate.800}',
                        //             900: '{slate.900}',
                        //             950: '{slate.950}',
                        //         },
                        //     },
                        //     dark: {
                        //         surface: {
                        //             0: '#ffffff',
                        //             50: '{slate.50}',
                        //             100: '{slate.100}',
                        //             200: '{slate.200}',
                        //             300: '{slate.300}',
                        //             400: '{slate.400}',
                        //             500: '{slate.500}',
                        //             600: '{slate.600}',
                        //             700: '{slate.700}',
                        //             800: '{slate.800}',
                        //             900: '{slate.900}',
                        //             950: '{slate.950}',
                        //         },
                        //     },
                        // },
                    },
                }),
                options: {
                    darkModeSelector: '.my-app-dark',
                },
            },
        }),
        provideStore<AppState>(
            {
                router: routerReducer,
                auth: authReducer,
                webSocket: webSocketReducer,
                roles: rolesReducer,
                users: usersReducer,
                profile: profileReducer,
                repositories: repositoriesReducer,
                repository: repositoryReducer,
            },
            { metaReducers: [saveStateReducer] },
        ),
        provideEffects(
            errorsEffects,
            notificationEffects,
            authEffects,
            webSocketEffects,
            rolessEffects,
            usersEffects,
            profileEffects,
            accessTokensEffects,
            repositoriesEffects,
            repositoryEffects,
            repositoryLabelsEffects,
            repositoryStatusesEffects,
            issuesEffects,
        ),
        provideRouterStore(),
        provideStoreDevtools({ maxAge: 25, logOnly: !isDevMode() }),
    ],
};
