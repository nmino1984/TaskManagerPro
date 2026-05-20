import { Injectable, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Notification, UnreadCountResponse } from '../models/notification.models';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private apiUrl = `${environment.apiUrl}/notifications`;
  private http = inject(HttpClient);

  notifications = signal<Notification[]>([]);
  unreadCount = signal(0);

  private pollingInterval: ReturnType<typeof setInterval> | null = null;

  load(): void {
    this.http.get<Notification[]>(this.apiUrl).subscribe({
      next: (data) => this.notifications.set(data),
      error: (err) => console.error('Error loading notifications:', err)
    });
  }

  loadUnreadCount(): void {
    this.http.get<UnreadCountResponse>(`${this.apiUrl}/unread`).subscribe({
      next: (data) => this.unreadCount.set(data.count),
      error: (err) => console.error('Error loading unread count:', err)
    });
  }

  markAsRead(id: number): void {
    this.http.patch<void>(`${this.apiUrl}/${id}/read`, {}).subscribe({
      next: () => this.reload(),
      error: (err) => console.error('Error marking notification as read:', err)
    });
  }

  markAllAsRead(): void {
    this.http.patch<void>(`${this.apiUrl}/read-all`, {}).subscribe({
      next: () => this.reload(),
      error: (err) => console.error('Error marking all notifications as read:', err)
    });
  }

  startPolling(): void {
    if (this.pollingInterval) return;

    this.load();
    this.loadUnreadCount();

    this.pollingInterval = setInterval(() => {
      this.load();
      this.loadUnreadCount();
    }, 30000);
  }

  stopPolling(): void {
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
      this.pollingInterval = null;
    }
  }

  private reload(): void {
    this.load();
    this.loadUnreadCount();
  }
}
