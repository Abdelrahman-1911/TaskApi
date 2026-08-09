using Swashbuckle.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var tasks = new List<TaskItem>
{
    new() { Id = 1, Title = "Learn .NET Minimal APIs", Done = true },
    new() { Id = 2, Title = "Build CRUD Endpoints", Done = false },
    new() { Id = 3, Title = "Publish to GitHub", Done = false }
};

app.MapGet("/", () => Results.Ok(new { name = "Task API", version = "1.0", endpoints = new[] { "/tasks", "/health" } })).WithName("GetRoot").WithOpenApi();
app.MapGet("/health", () => Results.Ok(new { status = "ok" })).WithName("GetHealth").WithOpenApi();

app.MapGet("/tasks", () => Results.Ok(tasks)).WithName("GetAllTasks").WithOpenApi();

app.MapGet("/tasks/{id:int}", (int id) =>
    tasks.FirstOrDefault(t => t.Id == id) is TaskItem t ? Results.Ok(t) : Results.NotFound(new { error = $"Task {id} not found" })
).WithName("GetTaskById").WithOpenApi();

app.MapPost("/tasks", (CreateTaskDto input) =>
{
    if (string.IsNullOrWhiteSpace(input.Title)) return Results.BadRequest(new { error = "Title is required" });
    int nextId = tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;
    var newTask = new TaskItem { Id = nextId, Title = input.Title.Trim(), Done = false };
    tasks.Add(newTask);
    return Results.Created($"/tasks/{newTask.Id}", newTask);
}).WithName("CreateTask").WithOpenApi();

app.MapPut("/tasks/{id:int}", (int id, UpdateTaskDto input) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound(new { error = $"Task {id} not found" });
    if (string.IsNullOrWhiteSpace(input.Title)) return Results.BadRequest(new { error = "Title cannot be empty" });
    task.Title = input.Title.Trim();
    task.Done = input.Done;
    return Results.Ok(task);
}).WithName("UpdateTask").WithOpenApi();

app.MapDelete("/tasks/{id:int}", (int id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null) return Results.NotFound(new { error = $"Task {id} not found" });
    tasks.Remove(task);
    return Results.NoContent();
}).WithName("DeleteTask").WithOpenApi();

app.Run();

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Done { get; set; }
}

public class CreateTaskDto { public string Title { get; set; } = string.Empty; }
public class UpdateTaskDto { public string Title { get; set; } = string.Empty; public bool Done { get; set; } }