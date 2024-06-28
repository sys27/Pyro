export class ResponseError {
    public constructor(private readonly _message: string) {}

    public get message(): string {
        return this._message;
    }
}

export type PyroResponse<T> = T | ResponseError;
