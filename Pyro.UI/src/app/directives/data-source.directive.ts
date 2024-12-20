/* eslint-disable @typescript-eslint/no-explicit-any */
import {
    ChangeDetectorRef,
    DestroyRef,
    Directive,
    Host,
    input,
    OnInit,
    Optional,
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { DataSourceState } from '@states/data-source.state';
import { DataView } from 'primeng/dataview';
import { Dropdown } from 'primeng/dropdown';
import { MultiSelect } from 'primeng/multiselect';
import { Table } from 'primeng/table';
import { Observable } from 'rxjs';

@Directive({
    selector:
        'p-table[dataSource], p-dataView[dataSource], p-dropdown[dataSource], p-multiSelect[dataSource]',
    standalone: true,
})
export class DataSourceDirective implements OnInit {
    public dataSource = input.required<Observable<DataSourceState<any>>>({
        alias: 'dataSource',
    });

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly changeDetectorRef: ChangeDetectorRef,
        @Host() @Optional() private readonly table?: Table,
        @Host() @Optional() private readonly dataView?: DataView,
        @Host() @Optional() private readonly dropdown?: Dropdown,
        @Host() @Optional() private readonly multiselect?: MultiSelect,
    ) {}

    public ngOnInit(): void {
        this.dataSource()
            .pipe(takeUntilDestroyed(this.destroyRef))
            .subscribe(dataSource => {
                this.setLoading(dataSource.loading);
                this.setDataSource(dataSource.data);
            });
    }

    private setLoading(loading: boolean): void {
        if (this.table) {
            this.table.loading = loading;
        } else if (this.dataView) {
            this.dataView.loading = loading;

            // needed because DataView has ChangeDetectionStrategy.OnPush
            this.changeDetectorRef.markForCheck();
        } else if (this.dropdown) {
            this.dropdown.loading = loading;
        } else if (this.multiselect) {
            this.multiselect.loading = loading;
        }
    }

    private setDataSource(dataSource: any[]): void {
        if (this.table) {
            this.table.value = dataSource;
        } else if (this.dataView) {
            this.dataView.value = dataSource;

            // needed because DataView has ChangeDetectionStrategy.OnPush
            this.changeDetectorRef.markForCheck();
        } else if (this.dropdown) {
            this.dropdown.options = dataSource;
        } else if (this.multiselect) {
            this.multiselect.options = dataSource;
        }
    }
}
