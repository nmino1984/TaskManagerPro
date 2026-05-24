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
import { MatTabsModule } from '@angular/material/tabs';

import { MilestoneService } from '../../../core/services/milestone.service';
import { Milestone, MilestoneCreateRequest, MilestoneUpdateRequest, MilestoneStatus } from '../../../core/models/milestone.models';
import { MilestoneHistoryListComponent } from '../milestone-history-list/milestone-history-list.component';

interface MilestoneFormData {
  mode: 'create' | 'edit';
  milestone?: Milestone;
  taskId: number;
}

@Component({
  selector: 'app-milestone-form',
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
    MatTabsModule,
    MilestoneHistoryListComponent
  ],
  templateUrl: './milestone-form.component.html',
  styleUrls: ['./milestone-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MilestoneFormComponent {
  private milestoneService = inject(MilestoneService);
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<MilestoneFormComponent>);
  data: MilestoneFormData = inject(MAT_DIALOG_DATA);

  form: FormGroup;
  loading = signal(false);
  error = signal<string | null>(null);
  MilestoneStatus = MilestoneStatus;
  statusOptions = Object.values(MilestoneStatus);

  constructor() {
    if (!this.data) {
      this.data = { mode: 'create', taskId: 0 };
    }

    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      description: ['', [Validators.required, Validators.minLength(5)]],
      targetDate: ['', Validators.required],
      status: [MilestoneStatus.Pending, Validators.required]
    });

    if (this.data.mode === 'edit' && this.data.milestone) {
      this.form.patchValue({
        title: this.data.milestone.title,
        description: this.data.milestone.description,
        targetDate: new Date(this.data.milestone.targetDate),
        status: this.data.milestone.status
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);
    this.error.set(null);

    if (this.data.mode === 'create') {
      const createRequest: MilestoneCreateRequest = {
        taskId: this.data.taskId,
        ...this.form.value
      };

      this.milestoneService.create(createRequest).subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.loading.set(false);
          const errorMsg = err.error?.detail || err.error?.message || 'Error creating milestone';
          this.error.set(errorMsg);
        }
      });
    } else if (this.data.mode === 'edit' && this.data.milestone) {
      const updateRequest: MilestoneUpdateRequest = this.form.value;

      this.milestoneService.update(this.data.milestone.milestoneId, updateRequest).subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close(true);
        },
        error: (err) => {
          this.loading.set(false);
          const errorMsg = err.error?.detail || err.error?.message || 'Error updating milestone';
          this.error.set(errorMsg);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
