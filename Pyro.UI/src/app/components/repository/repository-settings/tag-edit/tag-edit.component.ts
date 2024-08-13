import { Component, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { Color } from '@models/color';
import { mapErrorToNull } from '@services/operators';
import { TagService } from '@services/tag.service';
import { ButtonModule } from 'primeng/button';
import { ColorPickerModule } from 'primeng/colorpicker';
import { InputTextModule } from 'primeng/inputtext';

@Component({
    selector: 'tag-edit',
    standalone: true,
    imports: [ButtonModule, ColorPickerModule, InputTextModule, ReactiveFormsModule, RouterLink],
    templateUrl: './tag-edit.component.html',
    styleUrl: './tag-edit.component.css',
})
export class TagEditComponent {
    public readonly repositoryName = input.required<string>();
    public readonly tagId = input.required<string>();
    public readonly form = this.formBuilder.nonNullable.group({
        name: ['', [Validators.required, Validators.maxLength(50)]],
        color: [{} as Color, [Validators.required]],
    });
    public readonly isLoading = signal<boolean>(false);

    public constructor(
        private readonly formBuilder: FormBuilder,
        private readonly router: Router,
        private readonly route: ActivatedRoute,
        private readonly tagService: TagService,
    ) {}

    public ngOnInit(): void {
        this.tagService
            .getTag(this.repositoryName(), this.tagId())
            .pipe(mapErrorToNull)
            .subscribe(tag => {
                if (tag === null) {
                    return;
                }

                this.form.setValue({
                    name: tag.name,
                    color: tag.color,
                });
            });
    }

    public onSubmit(): void {
        if (this.form.invalid) {
            return;
        }

        this.isLoading.set(true);

        let tag = {
            name: this.form.value.name!,
            color: this.form.value.color!,
        };

        this.tagService
            .updateTag(this.repositoryName(), this.tagId(), tag)
            .pipe(mapErrorToNull)
            .subscribe(response => {
                if (response === null) {
                    this.isLoading.set(false);
                    return;
                }

                this.router.navigate(['../'], { relativeTo: this.route });
            });
    }
}
