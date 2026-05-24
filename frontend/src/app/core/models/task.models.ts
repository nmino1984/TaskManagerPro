export interface MyTask {
  myTaskId: number;
  title: string;
  description?: string;
  startDate: Date;
  endDate: Date;
  priority: TaskPriority;
  status: MyTaskStatus;
  progress: number;
  userId: string;
  assignedToUserId?: string;
  assignedToUsername?: string;
  createdAt: Date;
  updatedAt: Date;
  isDeleted: boolean;
  deletedAt?: Date;
}

export interface CreateTaskRequest {
  title: string;
  description?: string;
  startDate: Date;
  endDate: Date;
  priority: TaskPriority;
}

export interface UpdateTaskRequest {
  title: string;
  description?: string;
  startDate: Date;
  endDate: Date;
  priority: TaskPriority;
  status: MyTaskStatus;
  assignedToUserId?: string | null;
}

export enum TaskPriority {
  Low = 'Low',
  Medium = 'Medium',
  High = 'High'
}

export enum MyTaskStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Overdue = 'Overdue'
}
