import { Component, Inject, inject, signal, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatTabsModule } from '@angular/material/tabs';

import { TaskService } from '../../../core/services/task.service';
import { UserService } from '../../../core/services/user.service';
import { MyTask, TaskPriority, MyTaskStatus } from '../../../core/models/task.models';
import { User } from '../../../core/models/user.models';
import { SubTaskListComponent } from '../subtask-list/subtask-list.component';
import { MilestoneListComponent } from '../milestone-list/milestone-list.component';
import { TaskHistoryListComponent } from '../task-history-list/task-history-list.component';
import { TaskCommentListComponent } from '../task-comment-list/task-comment-list.component';

interface TaskFormData {
  mode: 'create' | 'edit';
  task?: MyTask;
}

@Component({
  selector: 'app-task-form',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTabsModule,
    SubTaskListComponent,
    MilestoneListComponent,
    TaskHistoryListComponent,
    TaskCommentListComponent
  ],
  templateUrl: './task-form.component.html',
  styleUrls: ['./task-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskFormComponent {
  private taskService = inject(TaskService);
  private userService = inject(UserService);
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<TaskFormComponent>);
  data: TaskFormData = inject(MAT_DIALOG_DATA);

  form: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);
  users = signal<User[]>([]);
  TaskPriority = TaskPriority;
  MyTaskStatus = MyTaskStatus;

  constructor() {
    if (!this.data) {
      this.data = { mode: 'create' };
    }

    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      startDate: [new Date(), Validators.required],
      endDate: [new Date(Date.now() + 7 * 24 * 60 * 60 * 1000), Validators.required],
      priority: [TaskPriority.Medium, Validators.required],
      status: [MyTaskStatus.NotStarted, Validators.required],
      assignedToUserId: [null]
    });

    if (this.data.mode === 'edit' && this.data.task) {
      this.form.patchValue({
        title: this.data.task.title,
        description: this.data.task.description,
        startDate: new Date(this.data.task.startDate),
        endDate: new Date(this.data.task.endDate),
        priority: this.data.task.priority,
        status: this.data.task.status,
        assignedToUserId: this.data.task.assignedToUserId || null
      });
    }

    this.loadUsers();
  }

  private loadUsers(): void {
    this.userService.getAll().subscribe({
      next: (response) => {
        this.users.set(response.items);
      },
      error: () => {
        // Silently fail on user load
      }
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.error.set(null);

    if (this.data.mode === 'create') {
      this.taskService.create(this.form.value).subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.loading.set(false);
          const errorMsg = err.error?.detail || err.error?.message || 'Error creating task';
          this.error.set(errorMsg);
        }
      });
    } else if (this.data.mode === 'edit' && this.data.task) {
      this.taskService.update(this.data.task.myTaskId, this.form.value).subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.loading.set(false);
          const errorMsg = err.error?.detail || err.error?.message || 'Error updating task';
          this.error.set(errorMsg);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
