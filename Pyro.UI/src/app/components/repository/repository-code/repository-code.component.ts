import { loadBranches, setBranchOrPath } from '@actions/repository.actions';
import { AsyncPipe, DatePipe, SlicePipe } from '@angular/common';
import { Component, computed, input, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { DataSourceDirective } from '@directives/data-source.directive';
import { Store } from '@ngrx/store';
import { MarkdownPipe } from '@pipes/markdown.pipe';
import { BranchItem, Repository, TreeView } from '@services/repository.service';
import { AppState } from '@states/app.state';
import { DataSourceState } from '@states/data-source.state';
import {
    selectBranches,
    selectBranchOrPath,
    selectDirectoryView,
    selectFile,
    selectFileContent,
    selectRepository,
    selectSelectedBranch,
} from '@states/repository.state';
import { ButtonModule } from 'primeng/button';
import { DeferModule } from 'primeng/defer';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputTextModule } from 'primeng/inputtext';
import { PopoverModule } from 'primeng/popover';
import { SelectModule } from 'primeng/select';
import { SkeletonModule } from 'primeng/skeleton';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview'; // TODO: replace
import { ToolbarModule } from 'primeng/toolbar';
import { combineLatest, distinctUntilChanged, filter, map, Observable, switchMap } from 'rxjs';

@Component({
    selector: 'repo-code',
    imports: [
        AsyncPipe,
        ButtonModule,
        DataSourceDirective,
        DatePipe,
        DeferModule,
        SelectModule,
        FormsModule,
        InputGroupModule,
        InputTextModule,
        MarkdownPipe,
        PopoverModule,
        RouterLink,
        SkeletonModule,
        SlicePipe,
        TableModule,
        TabViewModule,
        ToolbarModule,
    ],
    templateUrl: './repository-code.component.html',
    styleUrls: ['./repository-code.component.css'],
})
export class RepositoryCodeComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly cloneUrl = computed(() => {
        let localUrl = window.location.origin;

        return `${localUrl}/${this.repositoryName()}.git`;
    });
    public readonly directoryViewPlaceholder = Array.from({ length: 10 }).map(() => ({}));

    public readonly branchOrPath$: Observable<string[]> = this.store.select(selectBranchOrPath);
    public readonly repository$: Observable<Repository> = this.store
        .select(selectRepository)
        .pipe(filter(repository => !!repository));
    public readonly branches$: Observable<DataSourceState<BranchItem>> =
        this.store.select(selectBranches);
    public readonly selectedBranch$: Observable<BranchItem | null> = this.store
        .select(selectSelectedBranch)
        .pipe(filter(branch => !!branch));
    public readonly directoryView$: Observable<TreeView | null> =
        this.store.select(selectDirectoryView);
    public readonly readmeName$: Observable<string | null> = this.store.select(
        selectFile(['readme.md', 'readme.txt', 'readme']),
    );
    public readonly readmeFile$: Observable<string | null> = this.readmeName$.pipe(
        filter(readmeName => readmeName !== null),
        distinctUntilChanged(),
        switchMap(readmeName => this.store.select(selectFileContent(readmeName))),
    );
    public readonly licenseName$: Observable<string | null> = this.store.select(
        selectFile(['license.md', 'license.txt', 'license']),
    );
    public readonly licenseFile$: Observable<string | null> = this.licenseName$.pipe(
        filter(licenseName => licenseName !== null),
        distinctUntilChanged(),
        switchMap(licenseName => this.store.select(selectFileContent(licenseName))),
    );
    public readonly displayTabView$ = combineLatest([this.readmeName$, this.licenseName$]).pipe(
        map(([readmeName, licenseName]) => !!readmeName || !!licenseName),
    );

    public constructor(
        private readonly store: Store<AppState>,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
    ) {}

    public ngOnInit(): void {
        let repositoryName = this.repositoryName();
        this.store.dispatch(loadBranches({ repositoryName }));

        this.route.data.pipe(map(data => data['branchOrPath'])).subscribe(branchOrPath => {
            this.store.dispatch(setBranchOrPath({ branchOrPath }));
        });
    }

    public selectBranch(branch: BranchItem): void {
        this.router.navigate(['repositories', this.repositoryName(), 'code', branch.name], {
            replaceUrl: true,
        });
    }

    public copyCloneUrl(): void {
        navigator.clipboard.writeText(this.cloneUrl());
    }
}
