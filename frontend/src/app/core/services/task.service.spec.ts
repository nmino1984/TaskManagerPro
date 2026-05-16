import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { TaskService } from './task.service';

describe('TaskService', () => {
  let service: TaskService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [TaskService]
    });

    service = TestBed.inject(TaskService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should have getAll method', () => {
    expect(service.getAll).toBeDefined();
  });

  it('should have getById method', () => {
    expect(service.getById).toBeDefined();
  });

  it('should have create method', () => {
    expect(service.create).toBeDefined();
  });

  it('should have update method', () => {
    expect(service.update).toBeDefined();
  });

  it('should have delete method', () => {
    expect(service.delete).toBeDefined();
  });
});
