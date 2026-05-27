import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { TaskComment, TaskCommentCreateRequest, TaskCommentUpdateRequest } from '../models/task-comment.models';

@Injectable({
  providedIn: 'root'
})
export class TaskCommentService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/comments`;

  getByTask(taskId: number): Observable<TaskComment[]> {
    return this.http.get<TaskComment[]>(`${this.apiUrl}/bytask/${taskId}`);
  }

  create(request: TaskCommentCreateRequest): Observable<TaskComment> {
    return this.http.post<TaskComment>(this.apiUrl, request);
  }

  update(id: number, request: TaskCommentUpdateRequest): Observable<TaskComment> {
    return this.http.put<TaskComment>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
