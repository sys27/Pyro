import { CommonModule } from '@angular/common';
import { Component, OnInit, input } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { DropdownModule } from 'primeng/dropdown';
import { TableModule } from 'primeng/table';
import { TabViewModule } from 'primeng/tabview';
import { ToolbarModule } from 'primeng/toolbar';
import { BehaviorSubject, Observable, combineLatest, map, shareReplay, switchMap } from 'rxjs';
import { BranchItem, RepositoryService, TreeView } from '../../services/repository.service';

@Component({
    selector: 'repo-code',
    standalone: true,
    imports: [
        CommonModule,
        DropdownModule,
        FormsModule,
        RouterModule,
        TableModule,
        TabViewModule,
        ToolbarModule,
    ],
    templateUrl: './repository-code.component.html',
    styleUrls: ['./repository-code.component.css'],
})
export class RepositoryCodeComponent implements OnInit {
    public branchOrHash = input.required<string>({ alias: 'branchOrHash' });

    public repositoryName: Observable<string> | undefined;
    public selectedBranch = new BehaviorSubject<BranchItem | undefined>(undefined);
    public directoryView: Observable<TreeView> | undefined;
    public branches: Observable<BranchItem[]> | undefined;

    public constructor(
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly repositoryService: RepositoryService,
    ) {}

    public ngOnInit(): void {
        this.repositoryName = this.route.parent?.params.pipe(map(params => params['name']));
        this.branches = this.repositoryName?.pipe(
            switchMap(repositoryName => this.repositoryService.getBranches(repositoryName)),
            shareReplay(1), // TODO:
        );

        this.branches?.subscribe(branches => {
            let branch = branches.filter(b => b.name === this.branchOrHash());
            if (branch.length === 0) {
                return;
            }

            this.selectBranch(branch[0]);
        });

        // TODO: fix cancelation
        this.directoryView = combineLatest([this.repositoryName!, this.selectedBranch]).pipe(
            switchMap(([repositoryName, branch]) =>
                this.repositoryService.getTreeView(repositoryName, branch?.name),
            ),
            shareReplay(1),
        );
    }

    public selectBranch(branch: BranchItem): void {
        this.selectedBranch.next(branch);
        this.router.navigate(['../', branch.name], {
            relativeTo: this.route,
            queryParamsHandling: 'merge',
            replaceUrl: true,
        });
    }

    private hasFile(files: string[]): Observable<boolean> | undefined {
        return this.directoryView?.pipe(
            map(dv => dv.items.some(i => files.includes(i.name.toLowerCase()))),
        );
    }

    public hasReadmeFile(): Observable<boolean> | undefined {
        return this.hasFile(['readme.md', 'readme.txt', 'readme']);
    }

    public hasLicenseFile(): Observable<boolean> | undefined {
        return this.hasFile(['license.md', 'license.txt', 'license']);
    }
}
