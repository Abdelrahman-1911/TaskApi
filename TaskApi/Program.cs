using Swashbuckle.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// إضافة خدمات Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ----------------------------------------------------
// In-Memory Database (Stage 2)
// ----------------------------------------------------
var tasks = new List<TaskItem>
{
    new TaskItem { Id = 1, Title = "Learn .NET Minimal APIs", Done = true },
    new TaskItem { Id = 2, Title = "Build CRUD Endpoints", Done = false },
    new TaskItem { Id = 3, Title = "Publish to GitHub", Done = false }
};

// ----------------------------------------------------
// Stage 1: Root & Health Endpoints
// ----------------------------------------------------
app.MapGet("/", () => Results.Ok(new
{
    name = "Task API",
    version = "1.0",
    endpoints = new[] { "/tasks", "/health" }
}))
.WithName("GetRoot")
.WithOpenApi();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
.WithName("GetHealth")
.WithOpenApi();

// ----------------------------------------------------
// Stage 2: Read Endpoints (GET /tasks & GET /tasks/{id})
// ----------------------------------------------------

// 1. GET /tasks - عرض كل المهام
app.MapGet("/tasks", () => Results.Ok(tasks))
.WithName("GetAllTasks")
.WithOpenApi();

// 2. GET /tasks/{id} - عرض مهمة واحدة محددة
app.MapGet("/tasks/{id:int}", (int id) =>
{
    var task = tasks.FirstOrDefault(t => t.Id == id);
    if (task is null)
    {
        return Results.NotFound(new { error = $"Task {id} not found" });
    }
    return Results.Ok(task);
})
.WithName("GetTaskById")
.WithOpenApi();

app.Run();

// ----------------------------------------------------
// Task Model Class
// ----------------------------------------------------
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Done { get; set; }
}