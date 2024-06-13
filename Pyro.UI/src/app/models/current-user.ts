export class CurrentUser {
    public constructor(
        private readonly _id: number,
        private readonly _email: string,
        private readonly _roles: string[],
        private readonly _permissions: string[],
    ) { }

    public get id(): number {
        return this._id;
    }

    public get email(): string {
        return this._email;
    }
}
