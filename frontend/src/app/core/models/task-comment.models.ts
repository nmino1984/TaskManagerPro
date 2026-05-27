export interface TaskComment {
  taskCommentId: number;
  taskId: number;
  text: string;
  createdByUserId: string;
  createdByUsername: string | null;
  createdAt: string;
  updatedAt: string;
  isEdited: boolean;
  parentCommentId: number | null;
  replies: TaskComment[];
}

export interface TaskCommentCreateRequest {
  taskId: number;
  text: string;
  parentCommentId?: number | null;
}

export interface TaskCommentUpdateRequest {
  text: string;
}
