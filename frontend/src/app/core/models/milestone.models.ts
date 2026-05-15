export enum MilestoneStatus {
  Pending = 'Pending',
  Completed = 'Completed',
  Overdue = 'Overdue'
}

export interface Milestone {
  milestoneId: number;
  taskId: number;
  title: string;
  description: string;
  targetDate: Date;
  status: MilestoneStatus;
  createdAt: Date;
  updatedAt: Date;
}

export interface MilestoneCreateRequest {
  taskId: number;
  title: string;
  description: string;
  targetDate: Date;
  status?: MilestoneStatus;
}

export interface MilestoneUpdateRequest {
  title: string;
  description: string;
  targetDate: Date;
  status: MilestoneStatus;
}
