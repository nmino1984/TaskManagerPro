import { Component, Input, inject, signal, ChangeDetectionStrategy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTooltipModule } from '@angular/material/tooltip';

import { TaskCommentService } from '../../../core/services/task-comment.service';
import { TaskComment, TaskCommentCreateRequest } from '../../../core/models/task-comment.models';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-task-comment-list',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSnackBarModule,
    MatTooltipModule
  ],
  templateUrl: './task-comment-list.component.html',
  styleUrls: ['./task-comment-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class TaskCommentListComponent implements OnInit {
  @Input() taskId!: number;

  private commentService = inject(TaskCommentService);
  private authService = inject(AuthService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  comments = signal<TaskComment[]>([]);
  loading = signal(false);
  currentUserId = signal<string | null>(null);
  editingCommentId = signal<number | null>(null);
  replyingToId = signal<number | null>(null);
  form: FormGroup;

  constructor() {
    this.form = this.fb.group({
      text: ['', [Validators.required, Validators.minLength(1)]]
    });
  }

  ngOnInit(): void {
    const user = this.authService.currentUser();
    if (user) {
      this.currentUserId.set(user.userId);
    }
    this.loadComments();
  }

  private loadComments(): void {
    this.loading.set(true);
    this.commentService.getByTask(this.taskId).subscribe({
      next: (comments) => {
        this.comments.set(comments);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load comments', 'Close', { duration: 3000 });
      }
    });
  }

  onSubmit(): void {
    if (!this.form.valid || !this.taskId) return;

    const isEditing = this.editingCommentId();
    const request: TaskCommentCreateRequest = {
      taskId: this.taskId,
      text: this.form.get('text')?.value,
      parentCommentId: this.replyingToId() || undefined
    };

    if (isEditing) {
      this.commentService.update(isEditing, { text: request.text }).subscribe({
        next: () => {
          this.snackBar.open('Comment updated', 'Close', { duration: 2000 });
          this.resetForm();
          this.loadComments();
        },
        error: () => {
          this.snackBar.open('Failed to update comment', 'Close', { duration: 3000 });
        }
      });
    } else {
      this.commentService.create(request).subscribe({
        next: () => {
          this.snackBar.open('Comment posted', 'Close', { duration: 2000 });
          this.resetForm();
          this.loadComments();
        },
        error: () => {
          this.snackBar.open('Failed to post comment', 'Close', { duration: 3000 });
        }
      });
    }
  }

  onEdit(comment: TaskComment): void {
    if (comment.createdByUserId !== this.currentUserId()) return;
    this.editingCommentId.set(comment.taskCommentId);
    this.form.patchValue({ text: comment.text });
  }

  onDelete(id: number, comment: TaskComment): void {
    if (comment.createdByUserId !== this.currentUserId()) return;
    if (confirm('Are you sure you want to delete this comment?')) {
      this.commentService.delete(id).subscribe({
        next: () => {
          this.snackBar.open('Comment deleted', 'Close', { duration: 2000 });
          this.loadComments();
        },
        error: () => {
          this.snackBar.open('Failed to delete comment', 'Close', { duration: 3000 });
        }
      });
    }
  }

  onReply(parentId: number): void {
    this.replyingToId.set(parentId);
    this.editingCommentId.set(null);
    this.form.reset();
    setTimeout(() => {
      const textarea = document.querySelector('textarea');
      textarea?.focus();
    });
  }

  canEditOrDelete(comment: TaskComment): boolean {
    return comment.createdByUserId === this.currentUserId();
  }

  resetForm(): void {
    this.form.reset();
    this.editingCommentId.set(null);
    this.replyingToId.set(null);
  }

  getTimeAgo(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const seconds = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (seconds < 60) return 'just now';
    if (seconds < 3600) return Math.floor(seconds / 60) + 'm ago';
    if (seconds < 86400) return Math.floor(seconds / 3600) + 'h ago';
    return Math.floor(seconds / 86400) + 'd ago';
  }

  highlightMentions(text: string): string {
    return text.replace(/@(\w+)/g, '<span class="mention">@$1</span>');
  }
}
