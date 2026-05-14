import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { SubTask, SubTaskCreateRequest, SubTaskUpdateRequest } from '../models/subtask.models';

@Injectable({
  providedIn: 'root'
})
export class SubTaskService {
  private apiUrl = `${environment.apiUrl}/subtasks`;

  constructor(private http: HttpClient) {}

  getByTask(taskId: number): Observable<SubTask[]> {
    return this.http.get<SubTask[]>(`${this.apiUrl}/bytask/${taskId}`);
  }

  getById(id: number): Observable<SubTask> {
    return this.http.get<SubTask>(`${this.apiUrl}/${id}`);
  }

  create(request: SubTaskCreateRequest): Observable<SubTask> {
    return this.http.post<SubTask>(this.apiUrl, request);
  }

  update(id: number, request: SubTaskUpdateRequest): Observable<SubTask> {
    return this.http.put<SubTask>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
