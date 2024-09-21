/* eslint-disable @typescript-eslint/no-explicit-any */
import { DestroyRef, Directive, Host, input, OnInit, Optional } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { Dropdown } from 'primeng/dropdown';
import { MultiSelect } from 'primeng/multiselect';
import { finalize, Observable, switchMap, take } from 'rxjs';

@Directive({
    selector: 'p-dropdown[observableOptions], p-multiSelect[observableOptions]',
    standalone: true,
})
export class ObservableOptionsDirective implements OnInit {
    public observableOptions = input.required<Observable<any[]> | undefined>();
    private readonly observableOptions$ = toObservable(this.observableOptions);

    public constructor(
        private readonly destroyRef: DestroyRef,
        @Host() @Optional() private readonly dropdown?: Dropdown,
        @Host() @Optional() private readonly multiselect?: MultiSelect,
    ) {}

    public ngOnInit(): void {
        this.setLoading(true);
        this.observableOptions$
            .pipe(
                takeUntilDestroyed(this.destroyRef),
                switchMap(options => {
                    if (!options) {
                        return [];
                    }

                    return options;
                }),
                take(1),
                finalize(() => this.setLoading(false)),
            )
            .subscribe(options => this.setOptions(options));
    }

    private setLoading(loading: boolean): void {
        if (this.dropdown) {
            this.dropdown.loading = loading;
        } else if (this.multiselect) {
            this.multiselect.loading = loading;
        }
    }

    private setOptions(options: any[]): void {
        if (this.dropdown) {
            this.dropdown.options = options;
        } else if (this.multiselect) {
            this.multiselect.options = options;
        }
    }
}
