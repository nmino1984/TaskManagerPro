import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { MyTask, CreateTaskRequest, UpdateTaskRequest } from '../models/task.models';
import { PagedResult, TaskQueryParams } from '../models/paged-result.models';

@Injectable({
  providedIn: 'root'
})
export class TaskService {
  private apiUrl = `${environment.apiUrl}/tasks`;

  constructor(private http: HttpClient) {}

  getAll(params: TaskQueryParams): Observable<PagedResult<MyTask>> {
    let httpParams = new HttpParams()
      .set('page', params.page.toString())
      .set('pageSize', params.pageSize.toString());

    if (params.status) {
      httpParams = httpParams.set('status', params.status);
    }
    if (params.priority) {
      httpParams = httpParams.set('priority', params.priority);
    }
    if (params.search) {
      httpParams = httpParams.set('search', params.search);
    }

    return this.http.get<PagedResult<MyTask>>(this.apiUrl, { params: httpParams });
  }

  getById(id: number): Observable<MyTask> {
    return this.http.get<MyTask>(`${this.apiUrl}/${id}`);
  }

  create(request: CreateTaskRequest): Observable<MyTask> {
    return this.http.post<MyTask>(this.apiUrl, request);
  }

  update(id: number, request: UpdateTaskRequest): Observable<MyTask> {
    return this.http.put<MyTask>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  assign(id: number, assignToUserId: string | null): Observable<any> {
    return this.http.patch<any>(`${this.apiUrl}/${id}/assign`, { assignToUserId });
  }
}
