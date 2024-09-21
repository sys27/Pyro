import { inject } from '@angular/core';
import { CanActivateFn, RedirectCommand, Router } from '@angular/router';
import { PyroPermissions } from '@models/pyro-permissions';
import { Store } from '@ngrx/store';
import { AppState } from '@states/app.state';
import { selectCurrentUser } from '@states/auth.state';
import { map } from 'rxjs';

export function authGuard(args?: PyroPermissions | PyroPermissions[] | undefined): CanActivateFn {
    let permissions = args === undefined ? [] : Array.isArray(args) ? args : [args];

    return (route, state) => {
        let store = inject(Store<AppState>);
        let router = inject(Router);

        return store.select(selectCurrentUser).pipe(
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
