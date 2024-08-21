import { Pipe, PipeTransform } from '@angular/core';
import { Color } from '@models/color';

@Pipe({
    name: 'luminance',
    standalone: true,
    pure: true,
})
export class LuminanceColorPipe implements PipeTransform {
    public transform(value: Color): string {
        let luminance = this.calculateLuminance(value);

        return luminance > 0.5 ? 'black' : 'white';
    }

    private calculateLuminance(value: Color): number {
        let { r, g, b } = value;
        let a = [r, g, b].map(v => {
            v /= 255;

            return v <= 0.03928 ? v / 12.92 : Math.pow((v + 0.055) / 1.055, 2.4);
        });

        return 0.2126 * a[0] + 0.7152 * a[1] + 0.0722 * a[2];
    }
}
