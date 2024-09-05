import { AsyncPipe } from '@angular/common';
import { Component, DestroyRef, input, OnInit } from '@angular/core';
import { takeUntilDestroyed, toObservable } from '@angular/core/rxjs-interop';
import { FormGroupDirective } from '@angular/forms';
import { filter, map, Observable, of, switchMap, withLatestFrom } from 'rxjs';

@Component({
    selector: 'validation-summary',
    standalone: true,
    imports: [AsyncPipe],
    templateUrl: './validation-summary.component.html',
    styleUrl: './validation-summary.component.css',
})
export class ValidationSummaryComponent implements OnInit {
    public readonly controlName = input<string>();
    private readonly controlName$ = toObservable(this.controlName);
    public errors$: Observable<Error[]> | undefined;

    public constructor(
        private readonly destroyRef: DestroyRef,
        private readonly formGroup: FormGroupDirective,
    ) {}

    public ngOnInit(): void {
        this.errors$ = this.controlName$.pipe(
            takeUntilDestroyed(this.destroyRef),
            map(() => {
                let controlName = this.controlName();
                if (!controlName) {
                    return this.formGroup.form;
                }

                return this.formGroup.form.get(controlName);
            }),
            filter(control => !!control),
            switchMap(control => control.statusChanges.pipe(withLatestFrom(of(control)))),
            map(([status, control]) => {
                let controlErrors = control.errors;
                if (status !== 'INVALID' || control.pristine || !controlErrors) {
                    return [];
                }

                let errors = Object.keys(controlErrors).map(key => {
                    let message = controlErrors[key];

                    return { type: key, message };
                });

                return errors;
            }),
        );
    }
}

interface Error {
    type: string;
    message: string;
}
