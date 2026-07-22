using Microsoft.EntityFrameworkCore;
using TodoApp.Api.Data;
using TodoApp.Api.Services;
using Xunit;

namespace TodoApp.Api.Tests;

public class TodoServiceTests
{
    private static TodoService CreateService(out TodoDbContext context)
    {
        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        context = new TodoDbContext(options);
        return new TodoService(context);
    }

    [Fact]
    public async Task CreateAsync_AddsTodoAndReturnsIt()
    {
        var service = CreateService(out _);

        var todo = await service.CreateAsync("Buy milk");

        Assert.NotEqual(Guid.Empty, todo.Id);
        Assert.Equal("Buy milk", todo.Title);
        Assert.False(todo.IsCompleted);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllCreatedTodos()
    {
        var service = CreateService(out _);
        await service.CreateAsync("First");
        await service.CreateAsync("Second");

        var todos = await service.GetAllAsync();

        Assert.Equal(2, todos.Count);
        Assert.Contains(todos, t => t.Title == "First");
        Assert.Contains(todos, t => t.Title == "Second");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTodo_WhenExists()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync("Find me");

        var found = await service.GetByIdAsync(created.Id);

        Assert.NotNull(found);
        Assert.Equal("Find me", found!.Title);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenMissing()
    {
        var service = CreateService(out _);

        var found = await service.GetByIdAsync(Guid.NewGuid());

        Assert.Null(found);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesTitle_WhenExists()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync("Old title");

        var result = await service.UpdateAsync(created.Id, "New title");
        var updated = await service.GetByIdAsync(created.Id);

        Assert.True(result);
        Assert.Equal("New title", updated!.Title);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenMissing()
    {
        var service = CreateService(out _);

        var result = await service.UpdateAsync(Guid.NewGuid(), "New title");

        Assert.False(result);
    }

    [Fact]
    public async Task SetCompletedAsync_TogglesCompletion()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync("Finish me");

        var result = await service.SetCompletedAsync(created.Id, true);
        var updated = await service.GetByIdAsync(created.Id);

        Assert.True(result);
        Assert.True(updated!.IsCompleted);
    }

    [Fact]
    public async Task SetCompletedAsync_ReturnsFalse_WhenMissing()
    {
        var service = CreateService(out _);

        var result = await service.SetCompletedAsync(Guid.NewGuid(), true);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_RemovesTodo_WhenExists()
    {
        var service = CreateService(out _);
        var created = await service.CreateAsync("Delete me");

        var result = await service.DeleteAsync(created.Id);
        var found = await service.GetByIdAsync(created.Id);

        Assert.True(result);
        Assert.Null(found);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenMissing()
    {
        var service = CreateService(out _);

        var result = await service.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }
}
