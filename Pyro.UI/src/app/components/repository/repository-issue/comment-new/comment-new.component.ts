import { AsyncPipe } from '@angular/common';
import { Component, input, OnInit, output, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Comment, IssueService } from '@services/issue.service';
import { mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DividerModule } from 'primeng/divider';
import { InputTextModule } from 'primeng/inputtext';
import { PanelModule } from 'primeng/panel';
import { TabViewModule } from 'primeng/tabview';
import { MarkdownPipe } from '../../../../pipes/markdown.pipe';

@Component({
    selector: 'comment-new',
    standalone: true,
    imports: [
        AsyncPipe,
        ButtonModule,
        DividerModule,
        InputTextModule,
        MarkdownPipe,
        PanelModule,
        ReactiveFormsModule,
        TabViewModule,
    ],
    templateUrl: './comment-new.component.html',
    styleUrl: './comment-new.component.css',
})
export class CommentNewComponent implements OnInit {
    public readonly repositoryName = input.required<string>();
    public readonly issueNumber = input.required<number>();
    public readonly comment = input<Comment>();
    public readonly isLoading = signal<boolean>(false);
    public readonly form = this.formBuilder.group({
        comment: ['', [Validators.required, Validators.maxLength(2000)]],
    });
    public readonly commentAdded = output<Comment>();
    public readonly onCancel = output();

    public constructor(
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
                  this.issueNumber(),
                  this.comment()!.id,
                  comment,
              )
            : this.issueService.createIssueComment(
                  this.repositoryName(),
                  this.issueNumber(),
                  comment,
              );

        httpCall.pipe(mapErrorToNull).subscribe(comment => {
            this.isLoading.set(false);
            this.form.enable();

            if (comment) {
                this.commentAdded.emit(comment);
                this.form.reset();
            }
        });
    }

    public cancel(): void {
        this.onCancel.emit();
    }

    public get isEditMode(): boolean {
        return this.comment() !== undefined;
    }
}
