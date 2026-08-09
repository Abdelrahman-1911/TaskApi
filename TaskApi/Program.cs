using Swashbuckle.AspNetCore;
var builder = WebApplication.CreateBuilder(args);

// إضافة خدمات Swagger للتوثيق
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// تفعيل Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ----------------------------------------------------
// Stage 0 & Stage 1: Root & Health Endpoints
// ----------------------------------------------------

// 1. Root Endpoint: GET /
app.MapGet("/", () => Results.Ok(new
{
    name = "Task API",
    version = "1.0",
    endpoints = new[] { "/tasks", "/health" }
}))
.WithName("GetRoot")
.WithOpenApi();

// 2. Health Endpoint: GET /health
app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
.WithName("GetHealth")
.WithOpenApi();

app.Run();