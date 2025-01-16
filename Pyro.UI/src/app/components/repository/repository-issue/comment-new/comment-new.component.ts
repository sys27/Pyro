import { AsyncPipe } from '@angular/common';
import { Component, Injector, input, OnInit, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule } from '@angular/forms';
import { ValidationSummaryComponent, Validators } from '@controls/validation-summary';
import { MarkdownPipe } from '@pipes/markdown.pipe';
import { Comment, Issue, IssueService } from '@services/issue.service';
import { createErrorHandler } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { InputTextModule } from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';
import { TabViewModule } from 'primeng/tabview'; // TODO: replace

@Component({
    selector: 'comment-new',
    imports: [
        AsyncPipe,
        ButtonModule,
        DividerModule,
        InputTextModule,
        MarkdownPipe,
        PanelModule,
        ReactiveFormsModule,
        TabViewModule,
        ValidationSummaryComponent,
    ],
    templateUrl: './comment-new.component.html',
    styleUrl: './comment-new.component.css',
})
export class CommentNewComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly issue = input.required<Issue>();
    public readonly comment = input<Comment>();
    public readonly isLoading = signal<boolean>(false);
    public readonly form = this.formBuilder.group({
        comment: ['', [Validators.required('Comment'), Validators.maxLength('Comment', 2000)]],
    });
    public readonly commentAdded = output<Comment>();
    public readonly cancelled = output();

    public constructor(
        private readonly injector: Injector,
        private readonly formBuilder: FormBuilder,
        private readonly issueService: IssueService,
    ) {}

    public ngOnInit(): void {
        let comment = this.comment();
        if (comment) {
            this.form.setValue({
                comment: comment.content,
            });
        }
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let content = this.form.value.comment;
        if (!content || content.trim().length === 0) {
            return;
        }

        let comment = {
            content: content,
        };

        this.isLoading.set(true);
        this.form.disable();

        let httpCall = this.isEditMode
            ? this.issueService.updateIssueComment(
                  this.repositoryName(),
                  this.issue().issueNumber,
                  this.comment()!.id,
                  comment,
              )
            : this.issueService.createIssueComment(
                  this.repositoryName(),
                  this.issue().issueNumber,
                  comment,
              );

        httpCall.pipe(createErrorHandler(this.injector)).subscribe(comment => {
            this.isLoading.set(false);
            this.form.enable();

            if (comment) {
                this.commentAdded.emit(comment);
                this.form.reset();
            }
        });
    }

    public cancel(): void {
        this.cancelled.emit();
    }

    public get isEditMode(): boolean {
        return this.comment() !== undefined;
    }
}
