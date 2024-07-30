import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { mapErrorToNull } from '@services/operators';
import { Repository, RepositoryService } from '@services/repository.service';
import { Observable, combineLatest, from, map, of, shareReplay, switchMap } from 'rxjs';

@Component({
    selector: 'repository-file',
    standalone: true,
    imports: [CommonModule],
    templateUrl: './repository-file.component.html',
    styleUrls: ['./repository-file.component.css'],
})
export class RepositoryFileComponent implements OnInit {
    public branchOrPath$: Observable<string[]> | undefined;
    public repository$: Observable<Repository | null> | undefined;
    public fileContent$: Observable<string | null> | undefined;

    public constructor(
        private readonly route: ActivatedRoute,
        private readonly repositoryService: RepositoryService,
    ) {}

    public ngOnInit(): void {
        this.branchOrPath$ = this.route.params.pipe(map(params => Object.values(params)));
        this.repository$ = this.route.parent?.params.pipe(
            map(params => params['name']),
            switchMap(repositoryName => this.repositoryService.getRepository(repositoryName)),
            mapErrorToNull,
            shareReplay(1),
        );

        this.fileContent$ = combineLatest([this.repository$!, this.branchOrPath$!]).pipe(
            switchMap(([repository, branchOrPath]) =>
                this.repositoryService.getFile(repository!.name, branchOrPath.join('/')),
            ),
            mapErrorToNull,
            switchMap(blob => (blob != null ? from(blob.text()) : of(null))),
            shareReplay(1),
        );
    }
}
