import { Observable, map } from 'rxjs';
import { PyroResponse, ResponseError } from '../models/response';

export function mapErrorToNull<T>(source: Observable<PyroResponse<T>>): Observable<T | null> {
    return source.pipe(
        map(response => {
            if (response instanceof ResponseError) {
                return null;
            }

            return response;
        }),
    );
}

export function mapErrorToEmpty<T>(source: Observable<PyroResponse<T[]>>): Observable<T[]> {
    return source.pipe(
        map(response => {
            if (response instanceof ResponseError) {
                return [];
            }

            return response;
        }),
    );
}
