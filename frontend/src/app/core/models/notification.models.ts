export interface Notification {
  notificationId: number;
  title: string;
  message: string;
  type: string;
  relatedTaskId?: number;
  isRead: boolean;
  createdAt: string;
}

export interface UnreadCountResponse {
  count: number;
}
