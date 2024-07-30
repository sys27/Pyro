import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Observable, shareReplay } from 'rxjs';
import { AccessToken, AccessTokenService } from '@services/access-token.service';
import { mapErrorToEmpty } from '@services/operators';

@Component({
    selector: 'access-token-list',
    standalone: true,
    imports: [ButtonModule, CommonModule, RouterModule, TableModule],
    templateUrl: './access-token-list.component.html',
    styleUrl: './access-token-list.component.css',
})
export class AccessTokenListComponent implements OnInit {
    public accessTokens$: Observable<AccessToken[] | null> | undefined;

    public constructor(
        private readonly service: AccessTokenService,
        private readonly messageService: MessageService,
    ) {}

    public ngOnInit(): void {
        this.loadAccessTokens();
    }

    public deleteAccessToken(tokenName: string): void {
        this.service.deleteAccessToken(tokenName).subscribe(() => {
            this.messageService.add({
                severity: 'success',
                summary: 'Success',
                detail: `Access token ${tokenName} deleted`,
            });

            this.loadAccessTokens();
        });
    }

    private loadAccessTokens(): void {
        this.accessTokens$ = this.service.getAccessTokens().pipe(mapErrorToEmpty, shareReplay(1));
    }
}
