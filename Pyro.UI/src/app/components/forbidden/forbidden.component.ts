import { Component } from '@angular/core';
import { ButtonModule } from 'primeng/button';

@Component({
    selector: 'forbidden',
    standalone: true,
    imports: [ButtonModule],
    templateUrl: './forbidden.component.html',
    styleUrl: './forbidden.component.css',
})
export class ForbiddenComponent {}
