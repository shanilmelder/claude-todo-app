using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Models;

namespace TodoApp.Api.Services;

public class TodoService : ITodoService
{
    private readonly TodoDbContext _context;

    public TodoService(TodoDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Todo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Todos
            .OrderBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Todo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Todos.FindAsync([id], cancellationToken);
    }

    public async Task<Todo> CreateAsync(string title, CancellationToken cancellationToken = default)
    {
        var todo = new Todo
        {
            Id = Guid.NewGuid(),
            Title = title.Trim(),
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync(cancellationToken);

        return todo;
    }

    public async Task<bool> UpdateAsync(Guid id, string title, CancellationToken cancellationToken = default)
    {
        var todo = await _context.Todos.FindAsync([id], cancellationToken);
        if (todo is null)
        {
            return false;
        }

        todo.Title = title.Trim();
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> SetCompletedAsync(Guid id, bool isCompleted, CancellationToken cancellationToken = default)
    {
        var todo = await _context.Todos.FindAsync([id], cancellationToken);
        if (todo is null)
        {
            return false;
        }

        todo.IsCompleted = isCompleted;
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var todo = await _context.Todos.FindAsync([id], cancellationToken);
        if (todo is null)
        {
            return false;
        }

        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
