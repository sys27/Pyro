import { inject } from '@angular/core';
import { CanActivateFn, RedirectCommand, Router } from '@angular/router';
import { PyroPermissions } from '@models/pyro-permissions';
import { AuthService } from '@services/auth.service';
import { map } from 'rxjs';

export function authGuard(args?: PyroPermissions | PyroPermissions[] | undefined): CanActivateFn {
    let permissions = args === undefined ? [] : Array.isArray(args) ? args : [args];

    return (route, state) => {
        let authService = inject(AuthService);
        let router = inject(Router);

        return authService.currentUser.pipe(
            map(currentUser => {
                if (currentUser === null) {
                    let urlTree = router.createUrlTree(['/login'], {
                        queryParams: { returnUrl: state.url },
                    });
                    return new RedirectCommand(urlTree);
                }

                if (permissions.length > 0) {
                    let hasPermission = permissions.some(permission =>
                        currentUser.hasPermission(permission),
                    );
                    if (!hasPermission) {
                        let urlTree = router.createUrlTree(['/forbidden']);

                        return new RedirectCommand(urlTree);
                    }
                }

                return true;
            }),
        );
    };
}
