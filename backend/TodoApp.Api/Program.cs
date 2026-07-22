using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Dtos;
using TodoApp.Api.Models;
using TodoApp.Api.Services;

var builder = WebApplication.CreateBuilder(args);

const string CorsPolicy = "Frontend";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<TodoDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("TodoDb")));
}

builder.Services.AddScoped<ITodoService, TodoService>();

var app = builder.Build();

app.UseCors(CorsPolicy);

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<TodoDbContext>();
    db.Database.Migrate();
}

var todos = app.MapGroup("/api/todos");

todos.MapGet("/", async (ITodoService service) =>
{
    var items = await service.GetAllAsync();
    return Results.Ok(items.Select(ToResponse));
});

todos.MapGet("/{id:guid}", async (Guid id, ITodoService service) =>
{
    var todo = await service.GetByIdAsync(id);
    return todo is null ? Results.NotFound() : Results.Ok(ToResponse(todo));
});

todos.MapPost("/", async (CreateTodoRequest request, ITodoService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    var todo = await service.CreateAsync(request.Title);
    return Results.Created($"/api/todos/{todo.Id}", ToResponse(todo));
});

todos.MapPut("/{id:guid}", async (Guid id, UpdateTodoRequest request, ITodoService service) =>
{
    if (string.IsNullOrWhiteSpace(request.Title))
    {
        return Results.BadRequest("Title is required.");
    }

    var updated = await service.UpdateAsync(id, request.Title);
    return updated ? Results.NoContent() : Results.NotFound();
});

todos.MapPatch("/{id:guid}/complete", async (Guid id, SetCompletedRequest request, ITodoService service) =>
{
    var updated = await service.SetCompletedAsync(id, request.IsCompleted);
    return updated ? Results.NoContent() : Results.NotFound();
});

todos.MapDelete("/{id:guid}", async (Guid id, ITodoService service) =>
{
    var deleted = await service.DeleteAsync(id);
    return deleted ? Results.NoContent() : Results.NotFound();
});

app.Run();

static TodoResponse ToResponse(Todo todo) =>
    new(todo.Id, todo.Title, todo.IsCompleted, todo.CreatedAt);

public partial class Program { }
