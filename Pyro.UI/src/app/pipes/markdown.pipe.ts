import { Pipe, PipeTransform } from '@angular/core';
import { MarkdownService } from '@services/markdown.service';
import { Observable, of } from 'rxjs';

@Pipe({
    name: 'md',
    standalone: true,
})
export class MarkdownPipe implements PipeTransform {
    public constructor(private readonly markdownService: MarkdownService) {}

    public transform(value: string | null): Observable<string> {
        if (!value) {
            return of('');
        }

        return this.markdownService.parse(value);
    }
}
