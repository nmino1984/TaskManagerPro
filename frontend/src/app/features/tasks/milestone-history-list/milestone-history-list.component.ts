import { Component, Input, inject, signal, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';

import { AuditLogService } from '../../../core/services/audit-log.service';
import { MilestoneAuditLog } from '../../../core/models/audit-log.models';

@Component({
  selector: 'app-milestone-history-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatProgressSpinnerModule,
    MatDividerModule
  ],
  templateUrl: './milestone-history-list.component.html',
  styleUrls: ['./milestone-history-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class MilestoneHistoryListComponent implements OnInit {
  @Input() milestoneId!: number;

  private auditLogService = inject(AuditLogService);

  auditLogs = signal<MilestoneAuditLog[]>([]);
  loading = signal(false);
  displayedColumns = ['changedAt', 'action', 'fieldName', 'oldValue', 'newValue', 'changedByUsername'];

  ngOnInit(): void {
    this.loadHistory();
  }

  private loadHistory(): void {
    this.loading.set(true);
    this.auditLogService.getByMilestone(this.milestoneId).subscribe({
      next: (logs) => {
        this.auditLogs.set(logs);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }
}
