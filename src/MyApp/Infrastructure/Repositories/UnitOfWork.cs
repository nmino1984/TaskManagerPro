using MyApp.Application.Interfaces.Repositories;
using MyApp.Infrastructure.Data;

namespace MyApp.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private ITaskRepository? _taskRepository;
    private ISubTaskRepository? _subTaskRepository;
    private IMilestoneRepository? _milestoneRepository;
    private INotificationRepository? _notificationRepository;
    private IUserRepository? _userRepository;
    private IAuditLogRepository? _auditLogRepository;
    private ISubTaskAuditLogRepository? _subTaskAuditLogRepository;
    private IMilestoneAuditLogRepository? _milestoneAuditLogRepository;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public ITaskRepository Tasks => _taskRepository ??= new TaskRepository(_context);
    public ISubTaskRepository SubTasks => _subTaskRepository ??= new SubTaskRepository(_context);
    public IMilestoneRepository Milestones => _milestoneRepository ??= new MilestoneRepository(_context);
    public INotificationRepository Notifications => _notificationRepository ??= new NotificationRepository(_context);
    public IUserRepository Users => _userRepository ??= new UserRepository(_context);
    public IAuditLogRepository AuditLogs => _auditLogRepository ??= new AuditLogRepository(_context);
    public ISubTaskAuditLogRepository SubTaskAuditLogs => _subTaskAuditLogRepository ??= new SubTaskAuditLogRepository(_context);
    public IMilestoneAuditLogRepository MilestoneAuditLogs => _milestoneAuditLogRepository ??= new MilestoneAuditLogRepository(_context);

    public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
