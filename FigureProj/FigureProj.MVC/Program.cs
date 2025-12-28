using Microsoft.EntityFrameworkCore;
using FigureProj.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Додаємо сервіси до контейнера
builder.Services.AddControllersWithViews();

// Підключення до PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=localhost;Database=figuredb;Username=postgres;Password=1234";

builder.Services.AddDbContext<FigureContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Застосування міграцій при старті
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

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
