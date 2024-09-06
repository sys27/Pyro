import { AsyncPipe } from '@angular/common';
import { Component, Injector, input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { createErrorHandler } from '@services/operators';
import { Repository, RepositoryService } from '@services/repository.service';
import { combineLatest, filter, from, map, Observable, of, shareReplay, switchMap } from 'rxjs';

@Component({
    selector: 'repository-file',
    standalone: true,
    imports: [AsyncPipe],
    templateUrl: './repository-file.component.html',
    styleUrls: ['./repository-file.component.css'],
})
export class RepositoryFileComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public branchOrPath$: Observable<string[]> | undefined;
    public repository$: Observable<Repository | null> | undefined;
    public fileContent$: Observable<string | null> | undefined;

    public constructor(
        private readonly injector: Injector,
        private readonly route: ActivatedRoute,
        private readonly repositoryService: RepositoryService,
    ) {}

    public ngOnInit(): void {
        this.branchOrPath$ = this.route.data.pipe(map(data => data['branchOrPath']));
        this.repository$ = this.repositoryService
            .getRepository(this.repositoryName())
            .pipe(createErrorHandler(this.injector), shareReplay(1));

        this.fileContent$ = combineLatest([this.repository$!, this.branchOrPath$!]).pipe(
            filter(([repository, branchOrPath]) => repository != null && branchOrPath.length > 0),
            switchMap(([repository, branchOrPath]) =>
                this.repositoryService.getFile(repository!.name, branchOrPath.join('/')),
            ),
            createErrorHandler(this.injector),
            switchMap(blob => (blob != null ? from(blob.text()) : of(null))),
            shareReplay(1),
        );
    }
}
