import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TaskAuditLog, SubTaskAuditLog, MilestoneAuditLog } from '../models/audit-log.models';

@Injectable({
  providedIn: 'root'
})
export class AuditLogService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getByTask(taskId: number): Observable<TaskAuditLog[]> {
    return this.http.get<TaskAuditLog[]>(`${this.apiUrl}/tasks/${taskId}/history`);
  }

  getBySubTask(subTaskId: number): Observable<SubTaskAuditLog[]> {
    return this.http.get<SubTaskAuditLog[]>(`${this.apiUrl}/subtasks/${subTaskId}/history`);
  }

  getByMilestone(milestoneId: number): Observable<MilestoneAuditLog[]> {
    return this.http.get<MilestoneAuditLog[]>(`${this.apiUrl}/milestones/${milestoneId}/history`);
  }
}
