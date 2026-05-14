import { Component, Input, OnInit, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { SubTaskService } from '../../../core/services/subtask.service';
import { SubTask, SubTaskStatus } from '../../../core/models/subtask.models';
import { SubTaskFormComponent } from '../subtask-form/subtask-form.component';

@Component({
  selector: 'app-subtask-list',
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
  templateUrl: './subtask-list.component.html',
  styleUrls: ['./subtask-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SubTaskListComponent implements OnInit {
  @Input() taskId!: number;

  private subTaskService = inject(SubTaskService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  subTasks = signal<SubTask[]>([]);
  loading = signal(false);
  displayedColumns: string[] = ['description', 'status', 'dueDate', 'actions'];
  SubTaskStatus = SubTaskStatus;

  ngOnInit(): void {
    this.loadSubTasks();
  }

  loadSubTasks(): void {
    if (!this.taskId) return;

    this.loading.set(true);
    this.subTaskService.getByTask(this.taskId).subscribe({
      next: (subTasks) => {
        this.subTasks.set(subTasks);
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Error loading subtasks', 'Close', { duration: 2000 });
        this.loading.set(false);
      }
    });
  }

  openCreateDialog(): void {
    this.dialog.open(SubTaskFormComponent, {
      width: '400px',
      data: { mode: 'create', taskId: this.taskId }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.snackBar.open('Subtask created successfully', 'Close', { duration: 2000 });
        this.loadSubTasks();
      }
    });
  }

  editSubTask(subTask: SubTask): void {
    this.dialog.open(SubTaskFormComponent, {
      width: '400px',
      data: { mode: 'edit', subTask, taskId: this.taskId }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.snackBar.open('Subtask updated successfully', 'Close', { duration: 2000 });
        this.loadSubTasks();
      }
    });
  }

  deleteSubTask(subTask: SubTask): void {
    if (confirm(`Are you sure you want to delete this subtask?`)) {
      this.subTaskService.delete(subTask.subTaskId).subscribe({
        next: () => {
          this.snackBar.open('Subtask deleted successfully', 'Close', { duration: 2000 });
          this.loadSubTasks();
        },
        error: () => {
          this.snackBar.open('Error deleting subtask', 'Close', { duration: 2000 });
        }
      });
    }
  }

  getStatusColor(status: SubTaskStatus): string {
    return status === SubTaskStatus.Completed ? 'accent' : '';
  }

  formatDate(date?: Date): string {
    return date ? new Date(date).toLocaleDateString() : '-';
  }
}
