namespace TodoApp.Api.Dtos;

public record CreateTodoRequest(string Title);

public record UpdateTodoRequest(string Title);

public record SetCompletedRequest(bool IsCompleted);

public record TodoResponse(Guid Id, string Title, bool IsCompleted, DateTime CreatedAt);
