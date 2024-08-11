import { CommonModule } from '@angular/common';
import { Component, input, OnDestroy, OnInit, output } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import {
    BehaviorSubject,
    filter,
    map,
    Observable,
    of,
    shareReplay,
    Subject,
    switchMap,
    take,
    takeUntil,
} from 'rxjs';

@Component({
    selector: 'paginator',
    standalone: true,
    imports: [ButtonModule, CommonModule],
    templateUrl: './paginator.component.html',
    styleUrl: './paginator.component.css',
})
export class PaginatorComponent implements OnInit, OnDestroy {
    public readonly loader = input.required<LoaderFunc>();
    public readonly offsetSelector = input.required<OffsetSelector>();
    public readonly dataChanged = output<any[]>();

    private previous$: Observable<any[]> = of([]);
    private readonly current$: BehaviorSubject<any[]> = new BehaviorSubject<any[]>([]);
    private next$: Observable<any[]> = of([]);
    private readonly destroy$ = new Subject<void>();

    public ngOnInit(): void {
        this.loadAfter()
            .pipe(take(1))
            .subscribe(x => this.current$.next(x));
        this.previous$ = this.current$.pipe(
            filter(items => items.length > 0),
            switchMap(items => {
                let before: string | undefined;
                if (items.length > 0) {
                    let firstItem = items[0];
                    before = this.offsetSelector()(firstItem);
                }

                return this.loadBefore(before);
            }),
            shareReplay(1),
        );
        this.next$ = this.current$.pipe(
            filter(items => items.length > 0),
            switchMap(items => {
                let after: string | undefined;
                if (items.length > 0) {
                    let lastItem = items[items.length - 1];
                    after = this.offsetSelector()(lastItem);
                }

                return this.loadAfter(after);
            }),
            shareReplay(1),
        );

        this.current$
            .pipe(takeUntil(this.destroy$))
            .subscribe(items => this.dataChanged.emit(items));
    }

    public ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }

    private loadBefore(before?: string): Observable<any[]> {
        return this.loader()({ before: before });
    }

    private loadAfter(after?: string): Observable<any[]> {
        return this.loader()({ after: after });
    }

    public onPrevious(): void {
        this.previous$.pipe(take(1)).subscribe(x => this.current$.next(x));
    }

    public onNext(): void {
        this.next$.pipe(take(1)).subscribe(x => this.current$.next(x));
    }

    public get isPreviousEnabled(): Observable<boolean> {
        return this.previous$.pipe(map(items => items.length > 0));
    }

    public get isNextEnabled(): Observable<boolean> {
        return this.next$.pipe(map(items => items.length > 0));
    }
}

export interface PaginatorState {
    before?: string;
    after?: string;
}

export type LoaderFunc = (state: PaginatorState) => Observable<any[]>;

export type OffsetSelector = (item: any) => string;
