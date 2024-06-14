export interface ResponseError {
    message: string;
}

export type Response<T> = T | ResponseError;
