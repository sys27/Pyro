import { inject } from '@angular/core';
import { CanActivateFn, RedirectCommand, Router } from '@angular/router';
import { map } from 'rxjs';
import { AuthService } from './services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
    let authService = inject(AuthService);
    let router = inject(Router);

    return authService.currentUser.pipe(
        map(currentUser => {
            if (currentUser !== null) {
                return true;
            }

            let urlTree = router.createUrlTree(['/login'], {
                queryParams: { returnUrl: state.url },
            });
            return new RedirectCommand(urlTree);
        }),
    );
};
