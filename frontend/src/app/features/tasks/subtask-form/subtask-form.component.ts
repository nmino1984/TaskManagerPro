import { Component, Inject, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatTabsModule } from '@angular/material/tabs';

import { SubTaskService } from '../../../core/services/subtask.service';
import { SubTask, SubTaskStatus, SubTaskCreateRequest, SubTaskUpdateRequest } from '../../../core/models/subtask.models';
import { SubTaskHistoryListComponent } from '../subtask-history-list/subtask-history-list.component';

interface SubTaskFormData {
  mode: 'create' | 'edit';
  subTask?: SubTask;
  taskId: number;
}

@Component({
  selector: 'app-subtask-form',
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
    MatProgressSpinnerModule,
    MatDividerModule,
    MatTabsModule,
    SubTaskHistoryListComponent
  ],
  templateUrl: './subtask-form.component.html',
  styleUrls: ['./subtask-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SubTaskFormComponent {
  private subTaskService = inject(SubTaskService);
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<SubTaskFormComponent>);
  data: SubTaskFormData = inject(MAT_DIALOG_DATA);

  form: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);
  SubTaskStatus = SubTaskStatus;

  constructor() {
    if (!this.data) {
      this.data = { mode: 'create', taskId: 0 };
    }

    if (!this.data.taskId) {
      this.error.set('Error: Task ID is missing. Cannot create subtask.');
    }

    this.form = this.fb.group({
      description: ['', [Validators.required, Validators.minLength(5)]],
      status: [SubTaskStatus.Pending, Validators.required],
      dueDate: [null],
      notes: ['']
    });

    if (this.data.mode === 'edit' && this.data.subTask) {
      this.form.patchValue({
        description: this.data.subTask.description,
        status: this.data.subTask.status,
        dueDate: this.data.subTask.dueDate ? new Date(this.data.subTask.dueDate) : null,
        notes: this.data.subTask.notes
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    if (!this.data.taskId) {
      this.error.set('Task ID is missing. Cannot create subtask.');
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    if (this.data.mode === 'create') {
      const formValue = this.form.value;
      const createRequest: SubTaskCreateRequest = {
        taskId: this.data.taskId,
        description: formValue.description,
        status: (formValue.status?.toLowerCase()) as any || 'pending',
        dueDate: formValue.dueDate ? new Date(formValue.dueDate).toISOString() : null,
        notes: formValue.notes
      };

      this.subTaskService.create(createRequest).subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.loading.set(false);
          const errorMsg = err.error?.detail || err.error?.message || 'Error creating subtask';
          this.error.set(errorMsg);
        }
      });
    } else if (this.data.mode === 'edit' && this.data.subTask) {
      const formValue = this.form.value;
      const updateRequest: SubTaskUpdateRequest = {
        description: formValue.description,
        status: (formValue.status?.toLowerCase()) as any || 'pending',
        dueDate: formValue.dueDate ? new Date(formValue.dueDate).toISOString() : null,
        notes: formValue.notes
      };

      this.subTaskService.update(this.data.subTask.subTaskId, updateRequest).subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.loading.set(false);
          const errorMsg = err.error?.detail || err.error?.message || 'Error updating subtask';
          this.error.set(errorMsg);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
