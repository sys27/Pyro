import { UrlMatcher, UrlSegment } from '@angular/router';

type ParameterObject = {
    [name: string]: UrlSegment;
};

export function urlMatcher(prefix: string): UrlMatcher {
    return (segments, group, route) => {
        if (segments.length > 0 && segments[0].path === prefix) {
            let parameters: ParameterObject = {};
            for (let i = 1; i < segments.length; i++) {
                parameters[`parameter_${i}`] = segments[i];
            }

            return {
                consumed: segments,
                posParams: parameters,
            };
        }

        return null;
    };
}
