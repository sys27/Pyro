import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { ToolbarModule } from 'primeng/toolbar';
import { Observable } from 'rxjs';
import { DirectoryView, RepositoryService } from '../../services/repository.service';

@Component({
    selector: 'repo-code',
    standalone: true,
    imports: [CommonModule, TableModule, CardModule, ToolbarModule],
    templateUrl: './repository-code.component.html',
    styleUrls: ['./repository-code.component.css'],
})
export class RepositoryCodeComponent implements OnInit {
    public name = signal<string>('');
    public directoryView: Observable<DirectoryView> | undefined;

    public constructor(
        private readonly route: ActivatedRoute,
        private readonly repositoryService: RepositoryService,
    ) {}

    public ngOnInit(): void {
        this.route.parent?.params.subscribe(params => {
            this.name.set(params['name']);

            this.directoryView = this.repositoryService.getDirectoryView(this.name());
        });
    }
}
