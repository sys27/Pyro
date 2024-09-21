import { Injectable } from '@angular/core';
import DOMPurify from 'dompurify';
import hljs from 'highlight.js';
import { Marked } from 'marked';
import markedAlert from 'marked-alert';
import { markedHighlight } from 'marked-highlight';
import { Observable, from, map, of, switchMap } from 'rxjs';

@Injectable({
    providedIn: 'root',
})
export class MarkdownService {
    private readonly parser: Marked;

    public constructor() {
        this.parser = new Marked(
            {
                async: true,
                gfm: true,
            },
            markedAlert(),
            markedHighlight({
                async: true,
                langPrefix: 'hljs language-',
                highlight(code, lang, _) {
                    const language = hljs.getLanguage(lang) ? lang : 'plaintext';
                    return hljs.highlight(code, { language }).value;
                },
            }),
        );
    }

    public parse(markdown: string | Observable<string>): Observable<string> {
        if (markdown instanceof Observable) {
            return markdown.pipe(switchMap(md => this.parse(md)));
        }

        let html$: Observable<string>;
        let result = this.parser.parse(markdown);
        if (result instanceof Promise) {
            html$ = from(result);
        } else {
            html$ = of(result);
        }

        return html$.pipe(map(html => DOMPurify.sanitize(html)));
    }
}
