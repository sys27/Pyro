import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export class Validators {
    public static required(name: string): ValidatorFn {
        return control =>
            this.isEmptyInputValue(control.value)
                ? { required: `The '${name}' field is required` }
                : null;
    }

    public static minLength(name: string, length: number): ValidatorFn {
        return control => {
            if (this.isEmptyInputValue(control.value) || !this.hasValidLength(control.value)) {
                return null;
            }

            return control.value.length < length
                ? { minLength: `The '${name}' field must be at least ${length} characters long` }
                : null;
        };
    }

    public static maxLength(name: string, length: number): ValidatorFn {
        return control =>
            this.hasValidLength(control.value) && control.value.length > length
                ? { maxLength: `The '${name}' field must be at most ${length} characters long` }
                : null;
    }

    public static email(name: string): ValidatorFn {
        return control => {
            if (this.isEmptyInputValue(control.value)) {
                return null;
            }

            const EMAIL_REGEXP =
                /^(?=.{1,254}$)(?=.{1,64}@)[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;

            return EMAIL_REGEXP.test(control.value)
                ? null
                : { email: `The '${name}' field is invalid` };
        };
    }

    public static pattern(name: string, pattern: string | RegExp): ValidatorFn {
        if (!pattern) {
            return this.nullValidator;
        }

        let regex: RegExp;
        let regexStr: string;
        if (typeof pattern === 'string') {
            regexStr = '';

            if (pattern.charAt(0) !== '^') regexStr += '^';

            regexStr += pattern;

            if (pattern.charAt(pattern.length - 1) !== '$') regexStr += '$';

            regex = new RegExp(regexStr);
        } else {
            regexStr = pattern.toString();
            regex = pattern;
        }

        return control => {
            if (this.isEmptyInputValue(control.value)) {
                return null;
            }

            return regex.test(control.value) ? null : { pattern: `The '${name}' field is invalid` };
        };
    }

    private static isEmptyInputValue(value: any): boolean {
        return (
            value == null ||
            ((typeof value === 'string' || Array.isArray(value)) && value.length === 0)
        );
    }

    private static hasValidLength(value: any): boolean {
        return value != null && typeof value.length === 'number';
    }

    private static nullValidator(control: AbstractControl): ValidationErrors | null {
        return null;
    }
}
