export class CurrentUser {
    public constructor(
        private readonly _accessToken: string,
        private readonly _expiresIn: Date,
        private readonly _id: number,
        private readonly _email: string,
        private readonly _roles: string[],
        private readonly _permissions: string[],
    ) {}

    public get accessToken(): string {
        return this._accessToken;
    }

    public get expiresIn(): Date {
        return this._expiresIn;
    }

    public get id(): number {
        return this._id;
    }

    public get email(): string {
        return this._email;
    }
}
