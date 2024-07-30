import { Pipe, PipeTransform } from '@angular/core';
import { Observable } from 'rxjs';
import { MarkdownService } from '@services/markdown.service';

@Pipe({
    name: 'md',
    standalone: true,
})
export class MarkdownPipe implements PipeTransform {
    public constructor(private readonly markdownService: MarkdownService) {}

    public transform(value: any, ...args: any[]): Observable<string> {
        return this.markdownService.parse(value);
    }
}
