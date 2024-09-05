export class CurrentUser {
    public constructor(
        private readonly _accessToken: string,
        private readonly _expiresIn: Date,
        private readonly _id: string,
        private readonly _login: string,
        private readonly _roles: string[],
        private readonly _permissions: string[],
    ) {}

    public hasPermission(permission: string): boolean {
        return this._permissions.includes(permission);
    }

    public get accessToken(): string {
        return this._accessToken;
    }

    public get expiresIn(): Date {
        return this._expiresIn;
    }

    public get id(): string {
        return this._id;
    }

    public get login(): string {
        return this._login;
    }

    public get roles(): string[] {
        return this._roles;
    }

    public get permissions(): string[] {
        return this._permissions;
    }
}
