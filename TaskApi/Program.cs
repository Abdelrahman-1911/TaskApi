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
// In-Memory Database
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
// Stage 2: Read Endpoints
// ----------------------------------------------------
app.MapGet("/tasks", () => Results.Ok(tasks))
.WithName("GetAllTasks")
.WithOpenApi();

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

// ----------------------------------------------------
// Stage 3: Create Endpoint (POST /tasks)
// ----------------------------------------------------
app.MapPost("/tasks", (CreateTaskDto input) =>
{
    // Validation: التأكد إن العنوان مش فاضي
    if (string.IsNullOrWhiteSpace(input.Title))
    {
        return Results.BadRequest(new { error = "Title is required and cannot be empty" });
    }

    // توليد ID جديد تلقائياً
    int nextId = tasks.Any() ? tasks.Max(t => t.Id) + 1 : 1;

    var newTask = new TaskItem
    {
        Id = nextId,
        Title = input.Title.Trim(),
        Done = false
    };

    tasks.Add(newTask);

    // ارجاع 201 Created ومكان العنصر الجديد
    return Results.Created($"/tasks/{newTask.Id}", newTask);
})
.WithName("CreateTask")
.WithOpenApi();

app.Run();

// ----------------------------------------------------
// Models & DTOs
// ----------------------------------------------------
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool Done { get; set; }
}

// DTO لاستقبال بيانات الإنشاء فقط من العميل
public class CreateTaskDto
{
    public string Title { get; set; } = string.Empty;
}