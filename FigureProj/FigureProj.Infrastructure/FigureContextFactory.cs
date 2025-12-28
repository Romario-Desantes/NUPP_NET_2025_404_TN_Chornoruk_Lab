using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FigureProj.Infrastructure
{
    /// <summary>
    /// Factory для створення FigureContext під час виконання міграцій
    /// </summary>
    public class FigureContextFactory : IDesignTimeDbContextFactory<FigureContext>
    {
        public FigureContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FigureContext>();
            
            // Читаємо connection string з environment variables або appsettings
            var connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DEFAULTCONNECTION")
                ?? Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");
            
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            
            // Підтримка DATABASE_URL від Render (формат: postgresql://user:pass@host:port/dbname)
            if (string.IsNullOrEmpty(connectionString) && !string.IsNullOrEmpty(databaseUrl))
            {
                var uri = new Uri(databaseUrl);
                var userInfo = uri.UserInfo.Split(':');
                connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.LocalPath.TrimStart('/')};Username={userInfo[0]};Password={Uri.UnescapeDataString(userInfo[1])};SSL Mode=Require;Trust Server Certificate=true";
            }
            
            // Fallback для розробки (якщо нічого не встановлено)
            if (string.IsNullOrEmpty(connectionString))
            {
                // Спробуємо прочитати з appsettings.json
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile("appsettings.Development.json", optional: true)
                    .AddEnvironmentVariables()
                    .Build();
                
                connectionString = configuration.GetConnectionString("DefaultConnection");
            }
            
            // Останній fallback для локальної розробки
            if (string.IsNullOrEmpty(connectionString))
            {
                connectionString = "Host=localhost;Database=figuredb;Username=postgres;Password=postgres";
            }
            
            optionsBuilder.UseNpgsql(connectionString);

            return new FigureContext(optionsBuilder.Options);
        }
    }
}


