import { AsyncPipe } from '@angular/common';
import { Component, input, OnDestroy, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ResponseError } from '@models/response';
import { ColorPipe } from '@pipes/color.pipe';
import { mapErrorToEmpty } from '@services/operators';
import { Tag, TagService } from '@services/tag.service';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { TableModule } from 'primeng/table';
import { BehaviorSubject, Observable, shareReplay, switchMap } from 'rxjs';

@Component({
    selector: 'tag-list',
    standalone: true,
    imports: [AsyncPipe, ColorPipe, ButtonModule, RouterLink, TableModule],
    templateUrl: './tag-list.component.html',
    styleUrl: './tag-list.component.css',
})
export class TagListComponent implements OnInit, OnDestroy {
    public readonly repositoryName = input.required<string>();
    private readonly refreshTags$ = new BehaviorSubject<void>(undefined);
    public tags$: Observable<Tag[]> | undefined;

    public constructor(
        private readonly messageService: MessageService,
        private readonly tagService: TagService,
    ) {}

    public ngOnInit(): void {
        this.tags$ = this.refreshTags$.pipe(
            switchMap(() => this.tagService.getTags(this.repositoryName()).pipe(mapErrorToEmpty)),
            shareReplay(1),
        );
    }

    public ngOnDestroy(): void {
        this.refreshTags$.complete();
    }

    public deleteTag(tag: Tag): void {
        this.tagService.deleteTag(this.repositoryName(), tag.id).subscribe(response => {
            if (response instanceof ResponseError) {
                return;
            }

            this.messageService.add({
                severity: 'success',
                summary: 'Success',
                detail: `Tag ${tag.name} deleted`,
            });

            this.refreshTags$.next();
        });
    }

    // TODO:
    public getTagColor(tag: Tag): string {
        return `rgb(${tag.color.r}, ${tag.color.g}, ${tag.color.b})`;
    }
}
