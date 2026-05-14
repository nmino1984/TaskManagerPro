import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CalendarEvent, CalendarEventCreateRequest, CalendarEventUpdateRequest } from '../models/calendar-event.models';

@Injectable({
  providedIn: 'root'
})
export class CalendarEventService {
  private apiUrl = `${environment.apiUrl}/calendarevents`;

  constructor(private http: HttpClient) {}

  getByTask(taskId: number): Observable<CalendarEvent[]> {
    return this.http.get<CalendarEvent[]>(`${this.apiUrl}/bytask/${taskId}`);
  }

  getById(id: number): Observable<CalendarEvent> {
    return this.http.get<CalendarEvent>(`${this.apiUrl}/${id}`);
  }

  create(request: CalendarEventCreateRequest): Observable<CalendarEvent> {
    return this.http.post<CalendarEvent>(this.apiUrl, request);
  }

  update(id: number, request: CalendarEventUpdateRequest): Observable<CalendarEvent> {
    return this.http.put<CalendarEvent>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
