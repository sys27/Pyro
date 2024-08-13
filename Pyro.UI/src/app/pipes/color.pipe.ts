import { Pipe, PipeTransform } from '@angular/core';
import { Color } from '@models/color';

@Pipe({
    name: 'color',
    standalone: true,
    pure: true,
})
export class ColorPipe implements PipeTransform {
    public transform(value: Color): string {
        return `rgb(${value.r}, ${value.g}, ${value.b})`;
    }
}
