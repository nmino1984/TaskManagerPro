import { Component, Input, OnInit, inject, ChangeDetectionStrategy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatDialogModule, MatDialog } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { CalendarEventService } from '../../../core/services/calendar-event.service';
import { CalendarEvent } from '../../../core/models/calendar-event.models';
import { MyTaskStatus } from '../../../core/models/task.models';
import { CalendarEventFormComponent } from '../calendar-event-form/calendar-event-form.component';

@Component({
  selector: 'app-calendar-event-list',
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
  templateUrl: './calendar-event-list.component.html',
  styleUrls: ['./calendar-event-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class CalendarEventListComponent implements OnInit {
  @Input() taskId!: number;

  private calendarEventService = inject(CalendarEventService);
  private dialog = inject(MatDialog);
  private snackBar = inject(MatSnackBar);

  events = signal<CalendarEvent[]>([]);
  loading = signal(false);
  displayedColumns: string[] = ['date', 'status', 'actions'];

  ngOnInit(): void {
    this.loadEvents();
  }

  loadEvents(): void {
    if (!this.taskId) return;

    this.loading.set(true);
    this.calendarEventService.getByTask(this.taskId).subscribe({
      next: (events) => {
        this.events.set(events);
        this.loading.set(false);
      },
      error: () => {
        this.snackBar.open('Error loading calendar events', 'Close', { duration: 2000 });
        this.loading.set(false);
      }
    });
  }

  openCreateDialog(): void {
    this.dialog.open(CalendarEventFormComponent, {
      width: '400px',
      data: { mode: 'create', taskId: this.taskId }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.snackBar.open('Calendar event created successfully', 'Close', { duration: 2000 });
        this.loadEvents();
      }
    });
  }

  editEvent(event: CalendarEvent): void {
    this.dialog.open(CalendarEventFormComponent, {
      width: '400px',
      data: { mode: 'edit', event, taskId: this.taskId }
    }).afterClosed().subscribe((result) => {
      if (result) {
        this.snackBar.open('Calendar event updated successfully', 'Close', { duration: 2000 });
        this.loadEvents();
      }
    });
  }

  deleteEvent(event: CalendarEvent): void {
    if (confirm('Are you sure you want to delete this calendar event?')) {
      this.calendarEventService.delete(event.calendarEventId).subscribe({
        next: () => {
          this.snackBar.open('Calendar event deleted successfully', 'Close', { duration: 2000 });
          this.loadEvents();
        },
        error: () => {
          this.snackBar.open('Error deleting calendar event', 'Close', { duration: 2000 });
        }
      });
    }
  }

  getStatusColor(status: MyTaskStatus): string {
    switch (status) {
      case 'Completed':
        return 'accent';
      default:
        return '';
    }
  }

  formatDate(date: Date): string {
    return new Date(date).toLocaleDateString();
  }
}
