import { Component } from '@angular/core';
import { RepositoryService } from '../../services/repository.service';
import { ButtonModule } from 'primeng/button';

@Component({
    selector: 'app-repo-list',
    standalone: true,
    imports: [ButtonModule],
    templateUrl: './repository-list.component.html',
    styleUrl: './repository-list.component.css'
})
export class RepositoryListComponent {
    public constructor(private repoService: RepositoryService) { }
}
