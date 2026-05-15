import { Injectable } from '@angular/core';
import { HttpClient, HttpResponse } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Milestone, MilestoneCreateRequest, MilestoneUpdateRequest } from '../models/milestone.models';

@Injectable({
  providedIn: 'root'
})
export class MilestoneService {
  private apiUrl = `${environment.apiUrl}/milestones`;

  constructor(private http: HttpClient) {}

  getByTask(taskId: number): Observable<Milestone[]> {
    return this.http.get<Milestone[]>(`${this.apiUrl}/bytask/${taskId}`);
  }

  getById(id: number): Observable<Milestone> {
    return this.http.get<Milestone>(`${this.apiUrl}/${id}`);
  }

  create(request: MilestoneCreateRequest): Observable<Milestone> {
    return this.http.post<Milestone>(this.apiUrl, request);
  }

  update(id: number, request: MilestoneUpdateRequest): Observable<Milestone> {
    return this.http.put<Milestone>(`${this.apiUrl}/${id}`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  exportJson(taskId: number): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.apiUrl}/bytask/${taskId}/export/json`, {
      responseType: 'blob',
      observe: 'response'
    }) as Observable<HttpResponse<Blob>>;
  }

  exportXml(taskId: number): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.apiUrl}/bytask/${taskId}/export/xml`, {
      responseType: 'blob',
      observe: 'response'
    }) as Observable<HttpResponse<Blob>>;
  }

  exportIcal(taskId: number): Observable<HttpResponse<Blob>> {
    return this.http.get(`${this.apiUrl}/bytask/${taskId}/export/ical`, {
      responseType: 'blob',
      observe: 'response'
    }) as Observable<HttpResponse<Blob>>;
  }
}
