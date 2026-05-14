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

import { CalendarEventService } from '../../../core/services/calendar-event.service';
import { CalendarEvent, CalendarEventCreateRequest, CalendarEventUpdateRequest } from '../../../core/models/calendar-event.models';
import { MyTaskStatus } from '../../../core/models/task.models';

interface CalendarEventFormData {
  mode: 'create' | 'edit';
  event?: CalendarEvent;
  taskId: number;
}

@Component({
  selector: 'app-calendar-event-form',
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
    MatProgressSpinnerModule
  ],
  templateUrl: './calendar-event-form.component.html',
  styleUrls: ['./calendar-event-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CalendarEventFormComponent {
  private calendarEventService = inject(CalendarEventService);
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<CalendarEventFormComponent>);
  data: CalendarEventFormData = inject(MAT_DIALOG_DATA);

  form: FormGroup;
  loading = signal(false);
  MyTaskStatus = MyTaskStatus;
  statusOptions = Object.values(MyTaskStatus);

  constructor() {
    if (!this.data) {
      this.data = { mode: 'create', taskId: 0 };
    }

    this.form = this.fb.group({
      date: ['', Validators.required],
      status: ['', Validators.required]
    });

    if (this.data.mode === 'edit' && this.data.event) {
      this.form.patchValue({
        date: new Date(this.data.event.date),
        status: this.data.event.status
      });
    }
  }

  onSubmit(): void {
    if (this.form.invalid) return;

    this.loading.set(true);

    if (this.data.mode === 'create') {
      const createRequest: CalendarEventCreateRequest = {
        taskId: this.data.taskId,
        ...this.form.value
      };

      this.calendarEventService.create(createRequest).subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close(true);
        },
        error: () => {
          this.loading.set(false);
        }
      });
    } else if (this.data.mode === 'edit' && this.data.event) {
      const updateRequest: CalendarEventUpdateRequest = this.form.value;

      this.calendarEventService.update(this.data.event.calendarEventId, updateRequest).subscribe({
        next: () => {
          this.loading.set(false);
          this.dialogRef.close(true);
        },
        error: () => {
          this.loading.set(false);
        }
      });
    }
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }
}
