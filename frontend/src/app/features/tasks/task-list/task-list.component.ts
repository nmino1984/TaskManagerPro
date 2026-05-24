import { Component, OnInit, ViewChild, signal, computed, inject, DestroyRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormBuilder } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { debounceTime, Subject } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';

import { TaskService } from '../../../core/services/task.service';
import { MyTask, TaskPriority, MyTaskStatus } from '../../../core/models/task.models';
import { PagedResult, TaskQueryParams } from '../../../core/models/paged-result.models';
import { TaskFormComponent } from '../task-form/task-form.component';
import { NavbarComponent } from '../../../shared/navbar/navbar.component';

@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatTableModule,
    MatPaginatorModule,
    MatButtonModule,
    MatIconModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatChipsModule,
    MatDialogModule,
    MatSnackBarModule,
    MatToolbarModule,
    MatTooltipModule,
    NavbarComponent
  ],
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss']
})
export class TaskListComponent implements OnInit {
  private taskService = inject(TaskService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);
  private fb = inject(FormBuilder);

  tasks = signal<MyTask[]>([]);
  totalCount = signal(0);
  loading = signal(false);
  page = signal(1);
  pageSize = signal(10);

  searchTerm = signal('');
  selectedStatus = signal<string | null>(null);
  selectedPriority = signal<string | null>(null);

  displayedColumns: string[] = ['title', 'priority', 'assignedTo', 'status', 'progress', 'endDate', 'actions'];
  TaskPriority = TaskPriority;
  MyTaskStatus = MyTaskStatus;

  private searchSubject = new Subject<string>();

  constructor() {
    this.setupSearch();
  }

  ngOnInit(): void {
    this.loadTasks();
  }

  private setupSearch(): void {
    this.searchSubject.pipe(
      debounceTime(300),
      takeUntilDestroyed()
    ).subscribe(() => {
      this.page.set(1);
      this.loadTasks();
    });
  }

  loadTasks(): void {
    this.loading.set(true);

    const params: TaskQueryParams = {
      page: this.page(),
      pageSize: this.pageSize(),
      status: this.selectedStatus() || undefined,
      priority: this.selectedPriority() || undefined,
      search: this.searchTerm() || undefined
    };

    this.taskService.getAll(params).subscribe({
      next: (result: PagedResult<MyTask>) => {
        this.tasks.set(result.items);
        this.totalCount.set(result.totalCount);
        this.loading.set(false);
      },
      error: (err) => {
        this.snackBar.open('Error loading tasks', 'Close', { duration: 2000 });
        this.loading.set(false);
      }
    });
  }

  onSearch(value: string): void {
    this.searchTerm.set(value);
    this.searchSubject.next(value);
  }

  onStatusChange(status: string | null): void {
    this.selectedStatus.set(status);
    this.page.set(1);
    this.loadTasks();
  }

  onPriorityChange(priority: string | null): void {
    this.selectedPriority.set(priority);
    this.page.set(1);
    this.loadTasks();
  }

  onPageChange(event: PageEvent): void {
    this.page.set(event.pageIndex + 1);
    this.pageSize.set(event.pageSize);
    this.loadTasks();
  }

  openCreateDialog(): void {
    this.dialog.open(TaskFormComponent, {
      width: '900px',
      height: '600px',
      data: { mode: 'create' }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.snackBar.open('Task created successfully', 'Close', { duration: 2000 });
        this.loadTasks();
      }
    });
  }

  editTask(task: MyTask): void {
    this.dialog.open(TaskFormComponent, {
      width: '900px',
      height: '600px',
      data: { mode: 'edit', task }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.snackBar.open('Task updated successfully', 'Close', { duration: 2000 });
        this.loadTasks();
      }
    });
  }

  deleteTask(task: MyTask): void {
    if (confirm(`Are you sure you want to delete "${task.title}"?`)) {
      this.taskService.delete(task.myTaskId).subscribe({
        next: () => {
          this.snackBar.open('Task deleted successfully', 'Close', { duration: 2000 });
          this.loadTasks();
        },
        error: () => {
          this.snackBar.open('Error deleting task', 'Close', { duration: 2000 });
        }
      });
    }
  }

  getPriorityColor(priority: TaskPriority): string {
    switch (priority) {
      case TaskPriority.High:
        return 'warn';
      case TaskPriority.Medium:
        return 'accent';
      default:
        return '';
    }
  }

  getStatusColor(status: MyTaskStatus): string {
    switch (status) {
      case MyTaskStatus.Completed:
        return 'success';
      case MyTaskStatus.InProgress:
        return 'accent';
      case MyTaskStatus.Overdue:
        return 'warn';
      default:
        return '';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }
}
