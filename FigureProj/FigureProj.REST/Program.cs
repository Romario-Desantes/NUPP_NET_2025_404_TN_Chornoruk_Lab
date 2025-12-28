using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FigureProj.Infrastructure;
using FigureProj.Infrastructure.Repositories;
using FigureProj.Infrastructure.Services;
using FigureProj.Infrastructure.Models;
using FigureProj.Common.Services;
using FigureProj.Common.Models.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Додаємо контролери
builder.Services.AddControllers();

// Налаштування Swagger/OpenAPI з підтримкою JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FigureProj REST API",
        Version = "v1",
        Description = "REST API для роботи з геометричними фігурами (Лабораторна робота №5 - Identity)",
        Contact = new OpenApiContact
        {
            Name = "Роман Чорнорук",
            Email = "roman@example.com"
        }
    });

    // Додавання JWT аутентифікації в Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введіть JWT токен у форматі: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Підключення до PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

// Підтримка DATABASE_URL від Render (формат: postgresql://user:pass@host:port/dbname)
if (string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(databaseUrl))
{
    try
    {
        var uri = new Uri(databaseUrl);
        var userInfo = uri.UserInfo.Split(':');
        if (userInfo.Length >= 2)
        {
            var password = string.Join(":", userInfo.Skip(1)); // Обробляємо випадок, коли пароль містить ":"
            connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={Uri.UnescapeDataString(userInfo[0])};Password={Uri.UnescapeDataString(password)};SSL Mode=Require;Trust Server Certificate=true";
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠ Помилка парсингу DATABASE_URL: {ex.Message}");
        // Продовжуємо з іншими джерелами connection string
    }
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string не налаштований. Встановіть ConnectionStrings__DefaultConnection або DATABASE_URL");
}

builder.Services.AddDbContext<FigureContext>(options =>
    options.UseNpgsql(connectionString));

// Налаштування Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Налаштування паролів
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // Налаштування користувача
    options.User.RequireUniqueEmail = true;

    // Налаштування входу
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<FigureContext>()
.AddDefaultTokenProviders();

// Налаштування JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey не налаштований");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();

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

// Ініціалізація бази даних та ролей
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    
    try
    {
        // Застосування міграцій
        var dbContext = services.GetRequiredService<FigureContext>();
        await dbContext.Database.MigrateAsync();
        Console.WriteLine("✓ База даних готова до роботи");

        // Ініціалізація ролей
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedRolesAsync(roleManager);
        Console.WriteLine("✓ Ролі ініціалізовані");

        // Створення адміністратора (опціонально)
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var configuration = services.GetRequiredService<IConfiguration>();
        await SeedAdminUserAsync(userManager, configuration);
        Console.WriteLine("✓ Адміністратор створений");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"✗ Помилка ініціалізації: {ex.Message}");
        Console.WriteLine($"✗ Stack trace: {ex.StackTrace}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"✗ Inner exception: {ex.InnerException.Message}");
        }
    }
}

// Configure the HTTP request pipeline
// Swagger доступний в усіх середовищах (можна вимкнути через ENABLE_SWAGGER=false)
var enableSwagger = Environment.GetEnvironmentVariable("ENABLE_SWAGGER");
if (enableSwagger != "false")
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

// ВАЖЛИВО: Порядок middleware має значення!
app.UseAuthentication(); // Спочатку аутентифікація
app.UseAuthorization();  // Потім авторизація

app.MapControllers();

// Отримуємо порт для виводу в консоль (для Render PORT встановлюється автоматично)
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
var baseUrl = app.Environment.IsDevelopment() 
    ? $"http://localhost:{port}" 
    : $"http://0.0.0.0:{port}";

Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
Console.WriteLine("║           FigureProj REST API - Лабораторна робота №5         ║");
Console.WriteLine("║                  Identity & JWT Authentication                ║");
Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
if (enableSwagger != "false")
{
    Console.WriteLine($"→ Swagger UI: {baseUrl}");
}
Console.WriteLine($"→ API запущено на порту {port}");
Console.WriteLine("→ Документація API доступна на головній сторінці");
Console.WriteLine("→ Ролі: Administrator, Editor, Viewer");

app.Run();

// Метод для створення ролей
async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
{
    string[] roles = { "Administrator", "Editor", "Viewer" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
            Console.WriteLine($"  → Роль '{role}' створена");
        }
    }
}

// Метод для створення адміністратора за замовчуванням
async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
{
    var adminEmail = configuration["Admin:Email"] 
        ?? Environment.GetEnvironmentVariable("ADMIN__EMAIL");
    var adminPassword = configuration["Admin:Password"] 
        ?? Environment.GetEnvironmentVariable("ADMIN__PASSWORD");

    // Пропускаємо створення адміністратора, якщо не вказані credentials
    if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
    {
        Console.WriteLine("  → Створення адміністратора пропущено (не вказані Admin:Email/Admin:Password)");
        return;
    }
    
    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
    if (existingAdmin == null)
    {
        var adminUser = new ApplicationUser
        {
            UserName = adminEmail.Split('@')[0],
            Email = adminEmail,
            FullName = "System Administrator",
            CreatedAt = DateTime.UtcNow,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Administrator");
            Console.WriteLine($"  → Адміністратор створений: {adminEmail}");
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"  → Помилка створення адміністратора: {errors}");
        }
    }
    else
    {
        Console.WriteLine($"  → Адміністратор вже існує: {adminEmail}");
    }
}
