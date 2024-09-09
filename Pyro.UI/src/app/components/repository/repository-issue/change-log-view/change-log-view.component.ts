import { DatePipe } from '@angular/common';
import { Component, input } from '@angular/core';
import { TagComponent } from '@controls/tag/tag.component';
import { ColorPipe } from '@pipes/color.pipe';
import { LuminanceColorPipe } from '@pipes/luminance-color.pipe';
import { ChangeLogs, IssueChangeLogType, User } from '@services/issue.service';
import { DividerModule } from 'primeng/divider';
import { TooltipModule } from 'primeng/tooltip';

@Component({
    selector: 'change-log-view',
    standalone: true,
    imports: [ColorPipe, DatePipe, DividerModule, LuminanceColorPipe, TagComponent, TooltipModule],
    providers: [DatePipe],
    templateUrl: './change-log-view.component.html',
    styleUrl: './change-log-view.component.css',
})
export class ChangeLogViewComponent {
    public changeLog = input.required<ChangeLogs>();

    public constructor(private readonly datePipe: DatePipe) {}

    public getTooltipMessage(changeLog: ChangeLogs): string {
        return `Changed by ${changeLog.author.name} on ${this.datePipe.transform(changeLog.createdAt, 'short')}`;
    }

    public getAssigneeName(user: User | null): string {
        return user?.name ?? 'Unassigned';
    }

    public get issueChangeLogType(): typeof IssueChangeLogType {
        return IssueChangeLogType;
    }
}
