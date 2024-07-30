import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { IssueService, User } from '@services/issue.service';
import { mapErrorToEmpty, mapErrorToNull } from '@services/operators';
import { ButtonModule } from 'primeng/button';
import { DropdownModule } from 'primeng/dropdown';
import { InputTextModule } from 'primeng/inputtext';
import { filter, map, Observable, shareReplay, switchMap, take, withLatestFrom } from 'rxjs';

@Component({
    selector: 'repo-issue-new',
    standalone: true,
    imports: [ButtonModule, CommonModule, DropdownModule, InputTextModule, ReactiveFormsModule],
    templateUrl: './repository-issue-new.component.html',
    styleUrl: './repository-issue-new.component.css',
})
export class RepositoryIssueNewComponent implements OnInit {
    private repositoryName$: Observable<string> | undefined;
    private issueNumber$: Observable<number | undefined> | undefined;
    public isEditMode$: Observable<boolean> | undefined;
    public users$: Observable<User[]> | undefined;

    public form = this.formBuilder.group({
        title: ['', [Validators.required, Validators.maxLength(200)]],
        assigneeId: new FormControl<string | null>(null),
    });

    public isLoading: boolean = false;

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly issueService: IssueService,
    ) {}

    public ngOnInit(): void {
        this.repositoryName$ = this.route.parent?.params.pipe(map(params => params['name']));
        this.issueNumber$ = this.route.params.pipe(map(params => params['issueNumber']));
        this.isEditMode$ = this.issueNumber$.pipe(map(issueNumber => !!issueNumber));
        this.users$ = this.issueService.getUsers().pipe(mapErrorToEmpty, shareReplay(1));

        this.isEditMode$
            .pipe(
                filter(isEditMode => isEditMode),
                withLatestFrom(this.repositoryName$!, this.issueNumber$!),
                switchMap(([_, name, number]) => this.issueService.getIssue(name, number!)),
                mapErrorToNull,
                take(1),
            )
            .subscribe(issue => {
                if (issue) {
                    this.form.setValue({
                        title: issue.title,
                        assigneeId: issue.assignee?.id || null,
                    });
                }
            });
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        let issue = {
            title: this.form.value.title!,
            assigneeId: this.form.value.assigneeId!,
        };

        this.isLoading = true;

        this.isEditMode$
            ?.pipe(
                withLatestFrom(this.repositoryName$!, this.issueNumber$!),
                switchMap(([isEdit, name, issueNumber]) => {
                    let result;

                    if (isEdit) {
                        result = this.issueService.updateIssue(name, issueNumber!, issue);
                    } else {
                        result = this.issueService.createIssue(name, issue);
                    }

                    return result.pipe(map(() => [isEdit, name, issueNumber!]));
                }),
                take(1),
            )
            .subscribe(([isEdit, name, issueNumber]) => {
                if (isEdit) {
                    this.router.navigate(['repositories', name, 'issues', issueNumber]);
                } else {
                    this.router.navigate(['repositories', name, 'issues']);
                }

                this.isLoading = false;
            });
    }
}
