using Microsoft.EntityFrameworkCore;
using FigureProj.Infrastructure;
using FigureProj.Infrastructure.Repositories;
using FigureProj.Infrastructure.Services;
using FigureProj.Infrastructure.Models;
using FigureProj.Common.Services;
using FigureProj.Common.Models.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Додаємо контролери
builder.Services.AddControllers();

// Налаштування Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "FigureProj REST API",
        Version = "v1",
        Description = "REST API для роботи з геометричними фігурами (Лабораторна робота №4)",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Роман Чорнорук",
            Email = "roman@example.com"
        }
    });
});

// Підключення до PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=figuredb;Username=postgres;Password=1234";

builder.Services.AddDbContext<FigureContext>(options =>
    options.UseNpgsql(connectionString));

// Dependency Injection для репозиторіїв та сервісів
builder.Services.AddScoped<IRepository<FigureModel>, FigureRepository>();
builder.Services.AddScoped<ICrudServiceAsync<Figure>, DbCrudServiceAsync<Figure>>();

// CORS (якщо потрібно для фронтенду)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Застосування міграцій при старті (опціонально)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FigureContext>();
    try
    {
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("✓ База даних готова до роботи");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Помилка підключення до БД: {ex.Message}");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "FigureProj API v1");
        options.RoutePrefix = string.Empty; // Swagger на головній сторінці
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║           FigureProj REST API - Лабораторна робота №4         ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
Console.WriteLine($"→ Swagger UI: {(app.Environment.IsDevelopment() ? "http://localhost:5000" : "")}");
Console.WriteLine("→ Документація API доступна на головній сторінці");

app.Run();
