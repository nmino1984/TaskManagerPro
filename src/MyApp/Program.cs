using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MyApp.Api.Filters;
using MyApp.Api.Middleware;
using MyApp.Application.Interfaces;
using MyApp.Application.Mapping;
using MyApp.Application.Services;
using MyApp.Infrastructure.Data;
using Scalar.AspNetCore;
using Serilog;

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
            Description = "REST API for managing tasks, subtasks, and calendar events in a collaborative team environment.",
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
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddValidatorsFromAssemblyContaining<MyTaskValidator>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured"));

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
        policy.WithOrigins("http://localhost:4200", "http://127.0.0.1:8080", "http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISubTaskService, SubTaskService>();
builder.Services.AddScoped<IMilestoneService, MilestoneService>();

var app = builder.Build();

app.UseCors("Angular");

app.UseMiddleware<ErrorHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "TaskMaster Pro API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "TaskMaster Pro API";
    });

    app.MapScalarApiReference(options =>
    {
        options.Title = "TaskMaster Pro API";
        options.Theme = ScalarTheme.Purple;
    });
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
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

app.MapControllers();

app.Run();
