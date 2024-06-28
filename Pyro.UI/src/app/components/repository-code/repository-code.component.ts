import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { SkeletonModule } from 'primeng/skeleton';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { ToolbarModule } from 'primeng/toolbar';
import {
    BehaviorSubject,
    Observable,
    combineLatest,
    distinctUntilChanged,
    filter,
    map,
    shareReplay,
    switchMap,
} from 'rxjs';
import { mapErrorToEmpty, mapErrorToNull } from '../../services/operators';
import {
    BranchItem,
    Repository,
    RepositoryService,
    TreeView,
} from '../../services/repository.service';

@Component({
    selector: 'repo-code',
    standalone: true,
    imports: [
        ButtonModule,
        CommonModule,
        DropdownModule,
        FormsModule,
        RouterModule,
        SkeletonModule,
        TableModule,
        TabViewModule,
        ToolbarModule,
    ],
    templateUrl: './repository-code.component.html',
    styleUrls: ['./repository-code.component.css'],
})
export class RepositoryCodeComponent implements OnInit {
    public branchOrPath: Observable<string[]> | undefined;
    public repository: Observable<Repository | null> | undefined;
    public selectedBranch = new BehaviorSubject<BranchItem | undefined>(undefined);
    public directoryView: Observable<TreeView | null> | undefined;
    public branches: Observable<BranchItem[]> | undefined;
    public directoryViewPlaceholder: any[] = Array.from({ length: 10 }).map(() => ({}));

    public constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly repositoryService: RepositoryService,
    ) {}

    public ngOnInit(): void {
        this.branchOrPath = this.route.params.pipe(map(params => Object.values(params)));
        this.repository = this.route.parent?.params.pipe(
            map(params => params['name']),
            switchMap(repositoryName => this.repositoryService.getRepository(repositoryName)),
            mapErrorToNull,
            shareReplay(1),
        );
        this.branches = this.repository?.pipe(
            filter(repository => repository != null),
            switchMap(repository => this.repositoryService.getBranches(repository!.name)),
            mapErrorToEmpty,
            shareReplay(1),
        );

        combineLatest([this.branches!, this.branchOrPath!]).subscribe(
            ([branches, branchOrPath]) => {
                let branch = this.findBestBranch(branches, branchOrPath);

                this.selectedBranch.next(branch);
            },
        );

        this.directoryView = combineLatest([
            this.repository!.pipe(filter(repository => repository != null)),
            this.branchOrPath!,
        ]).pipe(
            distinctUntilChanged((prev, curr) => prev[0] === curr[0] && prev[1] === curr[1]),
            switchMap(([repository, branchOrPath]) =>
                this.repositoryService.getTreeView(repository!.name, branchOrPath.join('/')),
            ),
            mapErrorToNull,
            shareReplay(1),
        );
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
        this.selectedBranch.next(branch);
        this.repository?.subscribe(repository => {
            this.router.navigate(['repositories', repository?.name, 'code', branch.name], {
                replaceUrl: true,
            });
        });
    }

    private hasFile(files: string[]): Observable<boolean> | undefined {
        return this.directoryView?.pipe(
            map(dv => dv != null && dv.items.some(i => files.includes(i.name.toLowerCase()))),
        );
    }

    public hasReadmeFile(): Observable<boolean> | undefined {
        return this.hasFile(['readme.md', 'readme.txt', 'readme']);
    }

    public hasLicenseFile(): Observable<boolean> | undefined {
        return this.hasFile(['license.md', 'license.txt', 'license']);
    }
}
