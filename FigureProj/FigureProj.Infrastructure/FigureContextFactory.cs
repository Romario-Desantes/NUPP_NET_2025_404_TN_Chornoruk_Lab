using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

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
            
            // Connection string для PostgreSQL
            // Змініть параметри підключення відповідно до вашої БД
            optionsBuilder.UseNpgsql("Host=localhost;Database=figuredb;Username=postgres;Password=postgres");

            return new FigureContext(optionsBuilder.Options);
        }
    }
}


