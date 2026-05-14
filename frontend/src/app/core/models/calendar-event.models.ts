import { MyTaskStatus } from './task.models';

export interface CalendarEvent {
  calendarEventId: number;
  taskId: number;
  date: Date;
  status: MyTaskStatus;
}

export interface CalendarEventCreateRequest {
  taskId: number;
  date: Date;
  status?: MyTaskStatus;
}

export interface CalendarEventUpdateRequest {
  date: Date;
  status: MyTaskStatus;
}
