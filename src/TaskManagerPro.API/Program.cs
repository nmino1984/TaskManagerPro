using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using TaskManagerPro.API.Filters;
using TaskManagerPro.API.Middleware;
using TaskManagerPro.Application.Interfaces;
using TaskManagerPro.Application.Mapping;
using TaskManagerPro.Application.Services;
using TaskManagerPro.Application.Validators;
using TaskManagerPro.Infrastructure;
using TaskManagerPro.Infrastructure.Jobs;
using TaskManagerPro.Infrastructure.Persistence;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "TaskMaster Pro API",
            Version = "v1",
            Description = "REST API for managing tasks, subtasks, and milestones.",
            Contact = new OpenApiContact
            {
                Name = "TaskMaster Pro",
                Email = "dev@taskmasterpro.io"
            }
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddControllers(options =>
    {
        options.Filters.Add<ValidationFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

// Infrastructure: DbContext, UnitOfWork, JwtTokenService, BcryptPasswordHasher, Hangfire, TaskNotificationJob
builder.Services.AddInfrastructure(builder.Configuration);

// Application services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISubTaskService, SubTaskService>();
builder.Services.AddScoped<IMilestoneService, MilestoneService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITaskCommentService, TaskCommentService>();
builder.Services.AddScoped<IAuditLogService, AuditLogService>();

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
builder.Services.AddValidatorsFromAssemblyContaining<MyTaskCreateDtoValidator>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
    ?? (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Testing"
        ? "development-key-that-must-be-at-least-32-characters-long!!!"
        : throw new InvalidOperationException("JWT_KEY environment variable not configured"));
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS origins come from appsettings.Development.json → "CorsOrigins" array
// In production, set via env var or appsettings.Production.json
var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>()
    ?? ["http://localhost:4200"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins(corsOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// Global error handling — must be first to catch all exceptions
app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsProduction())
    app.UseHttpsRedirection();

// UseRouting must be explicit and before UseCors so CORS has route context for OPTIONS preflight
app.UseRouting();
app.UseCors("Angular");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TaskMaster Pro API v1");
        options.RoutePrefix = "swagger";
    });
    app.MapScalarApiReference(options =>
    {
        options.Title = "TaskMaster Pro API";
        options.Theme = ScalarTheme.Purple;
    });
    app.UseHangfireDashboard("/hangfire");
}

app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    try
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync();
        await DataSeeder.SeedAsync(db);
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error during database migration or seeding");
    }
}

// Register recurring job in all environments except test
if (!app.Environment.IsEnvironment("Testing"))
{
    RecurringJob.AddOrUpdate<TaskNotificationJob>(
        "task-overdue-check",
        j => j.TaskOverdueCheckAsync(),
        Cron.Hourly);
}

app.MapControllers();
app.Run();

public partial class Program { }
