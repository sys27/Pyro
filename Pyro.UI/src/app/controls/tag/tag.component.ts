import { Component, input } from '@angular/core';
import { Color } from '@models/color';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { TagModule } from 'primeng/tag';

@Component({
    selector: 'tag',
    standalone: true,
    imports: [ColorPipe, LuminanceColorPipe, TagModule],
    templateUrl: './tag.component.html',
    styleUrl: './tag.component.css',
})
export class TagComponent {
    public value = input.required<Tag>();
}

export interface Tag {
    name: string;
    color: Color;
}
