import { AsyncPipe } from '@angular/common';
import { AfterContentInit, Component, contentChild, OnDestroy } from '@angular/core';
import { toObservable } from '@angular/core/rxjs-interop';
import { FormControlName } from '@angular/forms';
import { filter, map, Observable, Subject, switchMap, takeUntil } from 'rxjs';

@Component({
    selector: 'with-validation',
    standalone: true,
    imports: [AsyncPipe],
    templateUrl: './with-validation.component.html',
    styleUrls: ['./with-validation.component.css'],
})
export class WithValidationComponent implements AfterContentInit, OnDestroy {
    public readonly formControlName = contentChild(FormControlName);
    public readonly formControlName$ = toObservable(this.formControlName);
    public errors$: Observable<Error[]> | undefined;
    private readonly destroy$ = new Subject<void>();

    private readonly map: Map<string, (error: any) => string> = new Map([
        ['required', _ => 'This field is required'],
        ['minlength', (error: LengthError) => `Minimum length is ${error.requiredLength}`],
        ['maxlength', (error: LengthError) => `Maximum length is ${error.requiredLength}`],
        ['pattern', _ => 'Invalid format'],
        ['email', _ => 'Invalid email'],
    ]);

    public ngAfterContentInit(): void {
        this.errors$ = this.formControlName$.pipe(
            takeUntil(this.destroy$),
            filter(control => !!control && !!control.statusChanges),
            switchMap(control => control!.statusChanges!),
            map(status => {
                let control = this.formControlName()!;
                let controlErrors = control.errors;
                if (status !== 'INVALID' || control.pristine || !controlErrors) {
                    return [];
                }

                let errors = Object.keys(controlErrors).map(key => {
                    let error = controlErrors[key];
                    let messageGetter = this.map.get(key);
                    if (!messageGetter) {
                        return { type: key, message: 'Invalid field' };
                    }

                    let message = messageGetter(error);

                    return { type: key, message };
                });

                return errors;
            }),
        );
    }

    public ngOnDestroy(): void {
        this.destroy$.next();
        this.destroy$.complete();
    }
}

interface Error {
    type: string;
    message: string;
}

interface LengthError {
    requiredLength: number;
    actualLength: number;
}
