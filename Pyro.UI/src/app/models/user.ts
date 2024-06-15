export interface User {
    get email(): string;

    get isLocked(): boolean;
}
