using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
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

builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ISubTaskService, SubTaskService>();
builder.Services.AddScoped<ICalendarEventService, CalendarEventService>();

var app = builder.Build();

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

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
