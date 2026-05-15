import { Component, Input, OnInit, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { MilestoneService } from '../../../core/services/milestone.service';
import { Milestone, MilestoneStatus } from '../../../core/models/milestone.models';
import { MilestoneFormComponent } from '../milestone-form/milestone-form.component';

@Component({
  selector: 'app-milestone-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatChipsModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './milestone-list.component.html',
  styleUrls: ['./milestone-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MilestoneListComponent implements OnInit {
  @Input() taskId!: number;

  private milestoneService = inject(MilestoneService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  milestones = signal<Milestone[]>([]);
  loading = signal(false);
  displayedColumns: string[] = ['title', 'targetDate', 'status', 'actions'];

  ngOnInit(): void {
    this.loadMilestones();
  }

  loadMilestones(): void {
    if (!this.taskId) return;

    this.loading.set(true);
    this.milestoneService.getByTask(this.taskId).subscribe({
      next: (milestones) => {
        this.milestones.set(milestones);
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Error loading milestones', 'Close', { duration: 2000 });
        this.loading.set(false);
      }
    });
  }

  openCreateDialog(): void {
    this.dialog.open(MilestoneFormComponent, {
      width: '500px',
      data: { mode: 'create', taskId: this.taskId }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.snackBar.open('Milestone created successfully', 'Close', { duration: 2000 });
        this.loadMilestones();
      }
    });
  }

  editMilestone(milestone: Milestone): void {
    this.dialog.open(MilestoneFormComponent, {
      width: '500px',
      data: { mode: 'edit', milestone, taskId: this.taskId }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.snackBar.open('Milestone updated successfully', 'Close', { duration: 2000 });
        this.loadMilestones();
      }
    });
  }

  deleteMilestone(milestone: Milestone): void {
    if (confirm('Are you sure you want to delete this milestone?')) {
      this.milestoneService.delete(milestone.milestoneId).subscribe({
        next: () => {
          this.snackBar.open('Milestone deleted successfully', 'Close', { duration: 2000 });
          this.loadMilestones();
        },
        error: () => {
          this.snackBar.open('Error deleting milestone', 'Close', { duration: 2000 });
        }
      });
    }
  }

  exportAs(format: 'json' | 'xml' | 'ical'): void {
    if (!this.taskId) return;

    const exportMethod =
      format === 'json' ? this.milestoneService.exportJson(this.taskId) :
      format === 'xml' ? this.milestoneService.exportXml(this.taskId) :
      this.milestoneService.exportIcal(this.taskId);

    exportMethod.subscribe({
      next: (response) => {
        const blob = response.body;
        const filename = `milestones.${format === 'ical' ? 'ics' : format}`;
        this.downloadFile(blob!, filename);
        this.snackBar.open(`Exported to ${filename}`, 'Close', { duration: 2000 });
      },
      error: () => {
        this.snackBar.open(`Error exporting to ${format}`, 'Close', { duration: 2000 });
      }
    });
  }

  private downloadFile(blob: Blob, filename: string): void {
    const url = window.URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    window.URL.revokeObjectURL(url);
  }

  getStatusColor(status: MilestoneStatus): string {
    switch (status) {
      case MilestoneStatus.Completed:
        return 'accent';
      case MilestoneStatus.Overdue:
        return 'warn';
      default:
        return '';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }
}
