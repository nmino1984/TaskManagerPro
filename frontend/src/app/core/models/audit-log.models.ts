export interface TaskAuditLog {
  taskAuditLogId: number;
  taskId: number;
  action: string;
  fieldName: string | null;
  oldValue: string | null;
  newValue: string | null;
  changedByUserId: string;
  changedByUsername: string | null;
  changedAt: string;
}

export interface SubTaskAuditLog {
  subTaskAuditLogId: number;
  subTaskId: number;
  action: string;
  fieldName: string | null;
  oldValue: string | null;
  newValue: string | null;
  changedByUserId: string;
  changedByUsername: string | null;
  changedAt: string;
}

export interface MilestoneAuditLog {
  milestoneAuditLogId: number;
  milestoneId: number;
  action: string;
  fieldName: string | null;
  oldValue: string | null;
  newValue: string | null;
  changedByUserId: string;
  changedByUsername: string | null;
  changedAt: string;
}
