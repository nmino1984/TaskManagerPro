using Microsoft.EntityFrameworkCore;
using MyApp.Infrastructure.Data;
using MyApp.Domain.Entities;

namespace MyApp.Api.Endpoints;

public static class MyTaskEndpoints
{
    public static void MapMyTaskEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/mytasks");

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.MyTasks
                .Include(t => t.SubTasks)
                .ToListAsync();
        });

        group.MapGet("/{id:int}", async (int id, AppDbContext db) =>
        {
            var task = await db.MyTasks
                .Include(t => t.SubTasks)
                .FirstOrDefaultAsync(t => t.MyTaskId == id);

            return task is null ? Results.NotFound() : Results.Ok(task);
        });

        group.MapPost("/", async (MyTask task, AppDbContext db) =>
        {
            db.MyTasks.Add(task);
            await db.SaveChangesAsync();
            return Results.Created($"/mytasks/{task.MyTaskId}", task);
        });

        group.MapPut("/{id:int}", async (int id, MyTask updatedTask, AppDbContext db) =>
        {
            var task = await db.MyTasks.FindAsync(id);
            if (task is null)
                return Results.NotFound();

            task.Title = updatedTask.Title;
            task.Description = updatedTask.Description;
            task.StartDate = updatedTask.StartDate;
            task.EndDate = updatedTask.EndDate;
            task.Priority = updatedTask.Priority;
            task.Status = updatedTask.Status;
            task.Progress = updatedTask.Progress;

            await db.SaveChangesAsync();
            return Results.Ok(task);
        });

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
