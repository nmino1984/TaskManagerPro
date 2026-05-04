using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MyApp.Application.DTOs;
using MyApp.Application.DTOs.CalendarEvent;
using MyApp.Application.DTOs.MyTask;
using MyApp.Application.DTOs.SubTask;
using MyApp.Domain.Entities;
using MyApp.Infrastructure.Data;

namespace MyApp.Api.Endpoints;

public static class MyTaskEndpoints
{
    public static void MapMyTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks");

        //
        // GET ALL
        //
        group.MapGet("/", async (AppDbContext db, IMapper mapper) =>
        {
            var tasks = await db.MyTasks
                .Include(t => t.SubTasks)
                .Include(t => t.CalendarEvents)
                .ToListAsync();

            var dto = mapper.Map<List<MyTaskResponseDto>>(tasks);
            return Results.Ok(dto);
        });

        //
        // GET BY ID
        //
        group.MapGet("/{id:int}", async (int id, AppDbContext db, IMapper mapper) =>
        {
            var task = await db.MyTasks
                .Include(t => t.SubTasks)
                .Include(t => t.CalendarEvents)
                .FirstOrDefaultAsync(t => t.MyTaskId == id);

            if (task is null)
                return Results.NotFound();

            var dto = mapper.Map<MyTaskResponseDto>(task);
            return Results.Ok(dto);
        });

        //
        // CREATE
        //
        group.MapPost("/", async (MyTaskCreateDto dto, AppDbContext db, IMapper mapper) =>
        {
            var task = mapper.Map<MyTask>(dto);

            task.CreatedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            db.MyTasks.Add(task);
            await db.SaveChangesAsync();

            var response = mapper.Map<MyTaskResponseDto>(task);
            return Results.Created($"/api/tasks/{task.MyTaskId}", response);
        });

        //
        // UPDATE
        //
        group.MapPut("/{id:int}", async (int id, MyTaskUpdateDto dto, AppDbContext db, IMapper mapper) =>
        {
            var task = await db.MyTasks.FindAsync(id);
            if (task is null)
                return Results.NotFound();

            mapper.Map(dto, task);

            task.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            var response = mapper.Map<MyTaskResponseDto>(task);
            return Results.Ok(response);
        });

        //
        // DELETE
        //
        group.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
        {
            var task = await db.MyTasks.FindAsync(id);
            if (task is null)
                return Results.NotFound();

            db.MyTasks.Remove(task);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}
