import { deleteAccessToken, loadAccessTokens } from '@actions/access-tokens.actions';
import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DataSourceDirective } from '@directives/data-source.directive';
import { Store } from '@ngrx/store';
import { AccessToken } from '@services/access-token.service';
import { AppState } from '@states/app.state';
import { DataSourceState } from '@states/data-source.state';
import { selectAccessTokens } from '@states/profile.state';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { Observable } from 'rxjs';

@Component({
    selector: 'access-token-list',
    standalone: true,
    imports: [ButtonModule, DataSourceDirective, DatePipe, RouterModule, TableModule],
    templateUrl: './access-token-list.component.html',
    styleUrl: './access-token-list.component.css',
})
export class AccessTokenListComponent implements OnInit {
    public accessTokens$: Observable<DataSourceState<AccessToken>> =
        this.store.select(selectAccessTokens);

    public constructor(private readonly store: Store<AppState>) {}

    public ngOnInit(): void {
        this.store.dispatch(loadAccessTokens());
    }

    public deleteAccessToken(tokenName: string): void {
        this.store.dispatch(deleteAccessToken({ tokenName }));
    }
}
