import { AsyncPipe, DatePipe, SlicePipe } from '@angular/common';
import { Component, computed, Injector, input, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { MarkdownService } from '@services/markdown.service';
import { createErrorHandler } from '@services/operators';
import { BranchItem, Repository, RepositoryService, TreeView } from '@services/repository.service';
import { ButtonModule } from 'primeng/button';
import { DeferModule } from 'primeng/defer';
import { DropdownModule } from 'primeng/dropdown';
import { InputGroupModule } from 'primeng/inputgroup';
import { InputTextModule } from 'primeng/inputtext';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { SkeletonModule } from 'primeng/skeleton';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { ToolbarModule } from 'primeng/toolbar';
import {
    BehaviorSubject,
    combineLatest,
    distinctUntilChanged,
    filter,
    from,
    map,
    Observable,
    of,
    shareReplay,
    Subject,
    switchMap,
    takeUntil,
    withLatestFrom,
} from 'rxjs';

@Component({
    selector: 'repo-code',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        DatePipe,
        DeferModule,
        DropdownModule,
        FormsModule,
        InputGroupModule,
        InputTextModule,
        OverlayPanelModule,
        RouterModule,
        SkeletonModule,
        SlicePipe,
        TableModule,
        TabViewModule,
        ToolbarModule,
    ],
    templateUrl: './repository-code.component.html',
    styleUrls: ['./repository-code.component.css'],
})
export class RepositoryCodeComponent implements OnInit, OnDestroy {
    public readonly repositoryName = input.required<string>();
    public readonly cloneUrl = computed(() => {
        let localUrl = window.location.origin;

        return `${localUrl}/${this.repositoryName()}.git`;
    });
    public branchOrPath$: Observable<string[]> | undefined;
    public repository$: Observable<Repository | null> | undefined;
    public readonly selectedBranch$ = new BehaviorSubject<BranchItem | undefined>(undefined);
    public directoryView$: Observable<TreeView | null> | undefined;
    public branches$: Observable<BranchItem[]> | undefined;
    public readmeName$: Observable<string | null> | undefined;
    public readmeFile$: Observable<string | null> | undefined;
    public licenseName$: Observable<string | null> | undefined;
    public licenseFile$: Observable<string | null> | undefined;
    public displayTabView$: Observable<boolean> | undefined;

    public readonly directoryViewPlaceholder: any[] = Array.from({ length: 10 }).map(() => ({}));

    private readonly destroy$ = new Subject<void>();

    public constructor(
        private readonly injector: Injector,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly repositoryService: RepositoryService,
        private readonly markdownService: MarkdownService,
    ) {}

    public ngOnInit(): void {
        this.branchOrPath$ = this.route.data.pipe(map(data => data['branchOrPath']));
        this.repository$ = this.repositoryService
            .getRepository(this.repositoryName())
            .pipe(createErrorHandler(this.injector), shareReplay(1));
        this.branches$ = this.repository$?.pipe(
            filter(repository => repository != null),
            switchMap(repository => this.repositoryService.getBranches(repository!.name)),
            createErrorHandler(this.injector),
            shareReplay(1),
        );

        combineLatest([this.branches$!, this.branchOrPath$!])
            .pipe(takeUntil(this.destroy$))
            .subscribe(([branches, branchOrPath]) => {
                let branch = this.findBestBranch(branches, branchOrPath);

                this.selectedBranch$.next(branch);
            });

        this.directoryView$ = combineLatest([
            this.repository$!.pipe(filter(repository => repository != null)),
            this.branchOrPath$!,
        ]).pipe(
            distinctUntilChanged((prev, curr) => prev[0] === curr[0] && prev[1] === curr[1]),
            switchMap(([repository, branchOrPath]) =>
                this.repositoryService.getTreeView(repository!.name, branchOrPath.join('/')),
            ),
            createErrorHandler(this.injector),
            shareReplay(1),
        );

        this.readmeName$ = this.hasFile(['readme.md', 'readme.txt', 'readme']);
        this.readmeFile$ = this.getMarkdownFile(this.readmeName$!);
        this.licenseName$ = this.hasFile(['license.md', 'license.txt', 'license']);
        this.licenseFile$ = this.getMarkdownFile(this.licenseName$!);

        this.displayTabView$ = combineLatest([this.readmeName$!, this.licenseName$!]).pipe(
            map(([readmeName, licenseName]) => readmeName != null || licenseName != null),
        );
    }

    public ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    private findBestBranch(branches: BranchItem[], branchOrPath: string[]): BranchItem {
        let end = 0;
        while (end < branchOrPath.length) {
            let branchName = branchOrPath.slice(0, end + 1).join('/');
            let branch = branches.find(b => b.name === branchName);
            if (branch != null) {
                return branch;
            }

            end++;
        }

        let defaultBranch = branches.find(b => b.isDefault);

        return defaultBranch ?? branches[0];
    }

    public selectBranch(branch: BranchItem): void {
        this.selectedBranch$.next(branch);
        this.repository$
            ?.pipe(
                filter(x => x !== null),
                takeUntil(this.destroy$),
            )
            .subscribe(repository => {
                this.router.navigate(['repositories', repository.name, 'code', branch.name], {
                    replaceUrl: true,
                });
            });
    }

    private hasFile(files: string[]): Observable<string | null> | undefined {
        return this.directoryView$?.pipe(
            map(dv => {
                if (dv == null) {
                    return null;
                }

                let file = dv.items.find(f => files.includes(f.name.toLowerCase()));
                if (file == null) {
                    return null;
                }

                return file.name;
            }),
        );
    }

    private getMarkdownFile(file: Observable<string | null>): Observable<string | null> {
        return file.pipe(
            withLatestFrom(this.repository$!, this.branchOrPath$!),
            switchMap(([fileName, repository, branchOrPath]) => {
                if (fileName == null) {
                    return of(null);
                }

                return this.repositoryService.getFile(
                    repository!.name,
                    branchOrPath.concat(fileName).join('/'),
                );
            }),
            createErrorHandler(this.injector),
            switchMap(blob => (blob != null ? from(blob.text()) : of(null))),
            switchMap(content => this.markdownService.parse(content ?? '')),
            shareReplay(1),
        );
    }

    public copyCloneUrl(): void {
        navigator.clipboard.writeText(this.cloneUrl());
    }
}
