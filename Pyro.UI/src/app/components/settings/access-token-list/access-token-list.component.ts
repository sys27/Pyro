import { AsyncPipe, DatePipe } from '@angular/common';
import { Component, Injector, OnDestroy, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AccessToken, AccessTokenService } from '@services/access-token.service';
import { createErrorHandler } from '@services/operators';

import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { BehaviorSubject, Observable, shareReplay, switchMap } from 'rxjs';

@Component({
    selector: 'access-token-list',
    standalone: true,
    imports: [AsyncPipe, ButtonModule, DatePipe, RouterModule, TableModule],
    templateUrl: './access-token-list.component.html',
    styleUrl: './access-token-list.component.css',
})
export class AccessTokenListComponent implements OnInit, OnDestroy {
    public accessTokens$: Observable<AccessToken[]> | undefined;
    private readonly refresh$ = new BehaviorSubject<void>(undefined);

    public constructor(
        private readonly injector: Injector,
        private readonly service: AccessTokenService,
        private readonly messageService: MessageService,
    ) {}

    public ngOnInit(): void {
        this.accessTokens$ = this.refresh$.pipe(
            switchMap(() => this.service.getAccessTokens().pipe(shareReplay(1))),
            createErrorHandler(this.injector),
        );
    }

    public ngOnDestroy(): void {
        this.refresh$.complete();
    }

    public deleteAccessToken(tokenName: string): void {
        this.service
            .deleteAccessToken(tokenName)
            .pipe(createErrorHandler(this.injector))
            .subscribe(() => {
                this.messageService.add({
                    severity: 'success',
                    summary: 'Success',
                    detail: `Access token ${tokenName} deleted`,
                });

                this.refresh$.next();
            });
    }
}
