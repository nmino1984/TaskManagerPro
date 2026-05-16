import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialogModule } from '@angular/material/dialog';
import { vi } from 'vitest';
import { TaskListComponent } from './task-list.component';
import { TaskService } from '../../../core/services/task.service';
import { of } from 'rxjs';
import { MyTaskStatus, TaskPriority } from '../../../core/models/task.models';

describe('TaskListComponent', () => {
  let component: TaskListComponent;
  let fixture: ComponentFixture<TaskListComponent>;
  let taskService: TaskService;

  beforeEach(async () => {
    // Create a mock TaskService with Vitest
    const mockTaskService = {
      getAll: vi.fn().mockReturnValue(of({})),
      getById: vi.fn().mockReturnValue(of({})),
      create: vi.fn().mockReturnValue(of({})),
      update: vi.fn().mockReturnValue(of({})),
      delete: vi.fn().mockReturnValue(of({}))
    };

    await TestBed.configureTestingModule({
      imports: [
        TaskListComponent,
        HttpClientTestingModule,
        MatSnackBarModule,
        MatDialogModule
      ],
      providers: [
        { provide: TaskService, useValue: mockTaskService }
      ]
    }).compileComponents();

    taskService = TestBed.inject(TaskService);
    fixture = TestBed.createComponent(TaskListComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with empty tasks', () => {
    expect(component.tasks()).toEqual([]);
    expect(component.loading()).toBe(false);
  });

  it('should load tasks on init', () => {
    const mockResponse = {
      items: [
        {
          myTaskId: 1,
          title: 'Test Task',
          description: 'Test Description',
          status: MyTaskStatus.NotStarted,
          priority: TaskPriority.Medium,
          startDate: new Date(),
          endDate: new Date(),
          progress: 0,
          userId: 'user-123',
          createdAt: new Date(),
          updatedAt: new Date(),
          isDeleted: false
        }
      ],
      totalCount: 1,
      page: 1,
      pageSize: 10
    };

    vi.mocked(taskService.getAll).mockReturnValue(of(mockResponse));

    fixture.detectChanges();

    expect(component.tasks().length).toBe(1);
    expect(component.tasks()[0].title).toBe('Test Task');
    expect(component.totalCount()).toBe(1);
  });

  it('should update search term', () => {
    component.searchTerm.set('new search');
    expect(component.searchTerm()).toBe('new search');
  });

  it('should update selected status', () => {
    component.selectedStatus.set('InProgress');
    expect(component.selectedStatus()).toBe('InProgress');
  });

  it('should update selected priority', () => {
    component.selectedPriority.set('High');
    expect(component.selectedPriority()).toBe('High');
  });

  it('should have correct displayed columns', () => {
    expect(component.displayedColumns).toContain('title');
    expect(component.displayedColumns).toContain('status');
    expect(component.displayedColumns).toContain('priority');
    expect(component.displayedColumns).toContain('actions');
  });

  it('should update page on pagination change', () => {
    const mockResponse = {
      items: [],
      totalCount: 50,
      page: 2,
      pageSize: 10
    };

    vi.mocked(taskService.getAll).mockReturnValue(of(mockResponse));

    component.onPageChange({ pageIndex: 1, pageSize: 10, length: 50 });

    expect(component.page()).toBe(2);
  });
});
