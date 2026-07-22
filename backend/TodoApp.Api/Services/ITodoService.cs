using TodoApp.Api.Models;

namespace TodoApp.Api.Services;

public interface ITodoService
{
    Task<IReadOnlyList<Todo>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Todo?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Todo> CreateAsync(string title, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(Guid id, string title, CancellationToken cancellationToken = default);

    Task<bool> SetCompletedAsync(Guid id, bool isCompleted, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
