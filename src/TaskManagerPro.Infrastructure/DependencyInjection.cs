using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Interfaces.Repositories;
using TaskManagerPro.Infrastructure.Jobs;
using TaskManagerPro.Infrastructure.Persistence;
using TaskManagerPro.Infrastructure.Persistence.Repositories;
using TaskManagerPro.Infrastructure.Services;

namespace TaskManagerPro.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<IBackgroundJobService, HangfireJobService>();
        services.AddScoped<TaskNotificationJob>();

        services.AddHangfire(config => config.UseMemoryStorage());
        services.AddHangfireServer();

        return services;
    }
}
