export interface SubTask {
  subTaskId: number;
  taskId: number;
  description: string;
  status: SubTaskStatus;
  dueDate?: Date;
  notes?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface SubTaskCreateRequest {
  taskId: number;
  description: string;
  status?: SubTaskStatus;
  dueDate?: string | null;
  notes?: string;
}

export interface SubTaskUpdateRequest {
  description: string;
  status: SubTaskStatus;
  dueDate?: string | null;
  notes?: string;
}

export enum SubTaskStatus {
  Pending = 'Pending',
  Completed = 'Completed'
}
