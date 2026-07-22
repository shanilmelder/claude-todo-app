using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TodoApp.Api.Data;
using TodoApp.Api.Dtos;
using Xunit;

namespace TodoApp.Api.Tests;

public class TodoEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TodoEndpointsTests(WebApplicationFactory<Program> factory)
    {
        var databaseName = Guid.NewGuid().ToString();
        var configuredFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<DbContextOptions<TodoDbContext>>();
                services.AddDbContext<TodoDbContext>(options =>
                    options.UseInMemoryDatabase(databaseName));
            });
        });

        _client = configuredFactory.CreateClient();
    }

    [Fact]
    public async Task PostThenGet_ReturnsCreatedTodo()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Write tests"));
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.NotNull(created);

        var getResponse = await _client.GetAsync($"/api/todos/{created!.Id}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var fetched = await getResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.Equal("Write tests", fetched!.Title);
        Assert.False(fetched.IsCompleted);
    }

    [Fact]
    public async Task Post_WithBlankTitle_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("   "));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ReturnsAllCreatedTodos()
    {
        await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Alpha"));
        await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Beta"));

        var response = await _client.GetAsync("/api/todos");
        var todos = await response.Content.ReadFromJsonAsync<List<TodoResponse>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(todos!.Count >= 2);
    }

    [Fact]
    public async Task Get_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.GetAsync($"/api/todos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Put_UpdatesTitle()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Original"));
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var putResponse = await _client.PutAsJsonAsync($"/api/todos/{created!.Id}", new UpdateTodoRequest("Renamed"));
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.Equal("Renamed", fetched!.Title);
    }

    [Fact]
    public async Task Put_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.PutAsJsonAsync($"/api/todos/{Guid.NewGuid()}", new UpdateTodoRequest("Renamed"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PatchComplete_TogglesCompletion()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Toggle me"));
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var patchResponse = await _client.PatchAsJsonAsync($"/api/todos/{created!.Id}/complete", new SetCompletedRequest(true));
        Assert.Equal(HttpStatusCode.NoContent, patchResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        var fetched = await getResponse.Content.ReadFromJsonAsync<TodoResponse>();
        Assert.True(fetched!.IsCompleted);
    }

    [Fact]
    public async Task Delete_RemovesTodo()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todos", new CreateTodoRequest("Delete me"));
        var created = await createResponse.Content.ReadFromJsonAsync<TodoResponse>();

        var deleteResponse = await _client.DeleteAsync($"/api/todos/{created!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/todos/{created.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_WithUnknownId_ReturnsNotFound()
    {
        var response = await _client.DeleteAsync($"/api/todos/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
