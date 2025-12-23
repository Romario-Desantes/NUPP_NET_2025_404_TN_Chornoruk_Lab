using FigureProj.Common.Models;
using FigureProj.Common.Models.Abstract;
using FigureProj.Infrastructure;
using FigureProj.Infrastructure.Services;
using FigureProj.Infrastructure.Repositories;
using FigureProj.Infrastructure.Models;
using FigureProj.NoSql.Repositories;
using FigureProj.NoSql.Models;
using FigureProj.NoSql.Mapping;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace FigureProj.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            System.Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            System.Console.WriteLine("║  ЛАБОРАТОРНА РОБОТА №3: РОБОТА З БАЗАМИ ДАНИХ                  ║");
            System.Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            // Налаштування підключення до PostgreSQL
            var connectionString = "Host=localhost;Database=figuredb;Username=postgres;Password=postgres";
            
            var optionsBuilder = new DbContextOptionsBuilder<FigureContext>();
            optionsBuilder.UseNpgsql(connectionString);

            using var context = new FigureContext(optionsBuilder.Options);

            System.Console.WriteLine("→ Підключення до бази даних PostgreSQL...");
            
            try
            {
                // Застосування міграцій та створення БД
                await context.Database.MigrateAsync();
                System.Console.WriteLine("  ✓ База даних готова до роботи\n");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"  ✗ Помилка підключення до БД: {ex.Message}");
                System.Console.WriteLine("\n  Переконайтеся, що PostgreSQL запущено та connection string правильний.");
                System.Console.WriteLine("  Connection string: " + connectionString);
                return;
            }

            // Створення репозиторію та сервісу
            var repository = new FigureRepository(context);
            var figureService = new DbCrudServiceAsync<Figure>(context, repository);

            System.Console.WriteLine("1. СТВОРЕННЯ ФІГУР ТА ЗБЕРЕЖЕННЯ В БД\n");

            // Створення фігур
            var figures = new List<Figure>
            {
                Circle.CreateNew(),
                Circle.CreateNew(),
                Rectangle.CreateNew(),
                Rectangle.CreateNew(),
                Square.CreateNew(),
                Square.CreateNew(),
                Triangle.CreateNew(),
                Triangle.CreateNew()
            };

            // Додавання фігур до БД
            foreach (var figure in figures)
            {
                await figureService.CreateAsync(figure);
                System.Console.WriteLine($"  • Додано: {figure.Name} ({figure.GetType().Name})");
            }

            System.Console.WriteLine($"\n  ✓ Всього додано до БД: {figures.Count} фігур\n");

            System.Console.WriteLine("2. ЧИТАННЯ ВСІХ ФІГУР З БД\n");

            var allFigures = (await figureService.ReadAllAsync()).ToList();
            System.Console.WriteLine($"→ Знайдено фігур у БД: {allFigures.Count}");
            foreach (var figure in allFigures.Take(5))
            {
                System.Console.WriteLine($"  • {figure.Name} - {figure.Color} (Площа: {figure.Area:F2})");
            }
            if (allFigures.Count > 5)
                System.Console.WriteLine($"  ... та ще {allFigures.Count - 5} фігур\n");
            else
                System.Console.WriteLine();

            System.Console.WriteLine("3. ДЕМОНСТРАЦІЯ ЗВ'ЯЗКІВ У БД\n");

            // Створення колекції (один-до-багатьох)
            var collection = new CollectionModel
            {
                Name = "Геометричні фігури",
                Description = "Основна колекція фігур",
                CreatedAt = DateTime.UtcNow
            };
            context.Collections.Add(collection);
            await context.SaveChangesAsync();
            System.Console.WriteLine($"  ✓ Створено колекцію: {collection.Name}");

            // Призначення фігур до колекції
            var dbFigures = await context.Figures.Take(3).ToListAsync();
            foreach (var dbFigure in dbFigures)
            {
                dbFigure.CollectionId = collection.Id;
            }
            await context.SaveChangesAsync();
            System.Console.WriteLine($"  ✓ Призначено {dbFigures.Count} фігур до колекції\n");

            // Створення тегів (багато-до-багатьох)
            var tags = new[]
            {
                new TagModel { Name = "Великі", Color = "червоний", CreatedAt = DateTime.UtcNow },
                new TagModel { Name = "Малі", Color = "синій", CreatedAt = DateTime.UtcNow },
                new TagModel { Name = "Середні", Color = "зелений", CreatedAt = DateTime.UtcNow }
            };
            context.Tags.AddRange(tags);
            await context.SaveChangesAsync();
            System.Console.WriteLine($"  ✓ Створено теги: {string.Join(", ", tags.Select(t => t.Name))}");

            // Призначення тегів фігурам
            var firstFigure = await context.Figures.FirstAsync();
            context.FigureTags.Add(new FigureTagModel 
            { 
                FigureId = firstFigure.Id, 
                TagId = tags[0].Id,
                AssignedAt = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            System.Console.WriteLine($"  ✓ Призначено тег '{tags[0].Name}' фігурі\n");

            // Створення метаданих (один-до-одного)
            var metadata = new FigureMetadataModel
            {
                FigureId = firstFigure.Id,
                Author = "Роман Чорнорук",
                Description = "Тестова фігура для демонстрації",
                CreatedBy = "System",
                LastModified = DateTime.UtcNow
            };
            context.FigureMetadata.Add(metadata);
            await context.SaveChangesAsync();
            System.Console.WriteLine($"  ✓ Створено метадані для фігури\n");

            System.Console.WriteLine("4. LINQ ЗАПИТИ ДО БАЗИ ДАНИХ\n");

            // Запит 1: Фігури за кольором
            var redFigures = await context.Figures
                .Where(f => f.Color.ToLower().Contains("червоний"))
                .ToListAsync();
            System.Console.WriteLine($"→ Червоних фігур: {redFigures.Count}");

            // Запит 2: Середня площа
            var avgArea = await context.Figures.AverageAsync(f => f.Area);
            System.Console.WriteLine($"→ Середня площа всіх фігур: {avgArea:F2}");

            // Запит 3: Найбільша фігура
            var largestFigure = await context.Figures
                .OrderByDescending(f => f.Area)
                .FirstOrDefaultAsync();
            if (largestFigure != null)
                System.Console.WriteLine($"→ Найбільша фігура: {largestFigure.Name} (Площа: {largestFigure.Area:F2})");

            // Запит 4: Кількість фігур за типом
            var circleCount = await context.Circles.CountAsync();
            var rectangleCount = await context.Rectangles.CountAsync();
            var squareCount = await context.Squares.CountAsync();
            var triangleCount = await context.Triangles.CountAsync();
            
            System.Console.WriteLine($"\n→ Статистика за типами:");
            System.Console.WriteLine($"  • Кіл: {circleCount}");
            System.Console.WriteLine($"  • Прямокутників: {rectangleCount}");
            System.Console.WriteLine($"  • Квадратів: {squareCount}");
            System.Console.WriteLine($"  • Трикутників: {triangleCount}\n");

            System.Console.WriteLine("5. ПАГІНАЦІЯ\n");

            int pageSize = 3;
            var page1 = await figureService.ReadAllAsync(1, pageSize);
            System.Console.WriteLine($"→ Сторінка 1 ({pageSize} елементів):");
            foreach (var figure in page1)
            {
                System.Console.WriteLine($"  • {figure.Name}");
            }

            var page2 = await figureService.ReadAllAsync(2, pageSize);
            System.Console.WriteLine($"\n→ Сторінка 2 ({pageSize} елементів):");
            foreach (var figure in page2)
            {
                System.Console.WriteLine($"  • {figure.Name}");
            }
            System.Console.WriteLine();

            System.Console.WriteLine("6. ОНОВЛЕННЯ ДАНИХ\n");

            var figureToUpdate = allFigures.First();
            var oldName = figureToUpdate.Name;
            figureToUpdate.Name = "Оновлена фігура";
            
            await figureService.UpdateAsync(figureToUpdate);
            System.Console.WriteLine($"  ✓ Оновлено фігуру: '{oldName}' → '{figureToUpdate.Name}'\n");

            System.Console.WriteLine("7. ВИДАЛЕННЯ ДАНИХ\n");

            var figureToDelete = allFigures.Last();
            var deletedName = figureToDelete.Name;
            
            await figureService.RemoveAsync(figureToDelete);
            System.Console.WriteLine($"  ✓ Видалено фігуру: '{deletedName}'\n");

            // Перевірка кількості після видалення
            var remainingCount = await context.Figures.CountAsync();
            System.Console.WriteLine($"  Залишилось фігур у БД: {remainingCount}\n");

            System.Console.WriteLine("8. СКЛАДНІ ЗАПИТИ З НАВІГАЦІЙНИМИ ВЛАСТИВОСТЯМИ\n");

            // Запит з включенням колекції
            var figuresWithCollection = await context.Figures
                .Include(f => f.Collection)
                .Where(f => f.CollectionId != null)
                .ToListAsync();
            
            System.Console.WriteLine($"→ Фігур у колекціях: {figuresWithCollection.Count}");
            foreach (var fig in figuresWithCollection)
            {
                System.Console.WriteLine($"  • {fig.Name} → Колекція: {fig.Collection?.Name}");
            }

            // Запит з включенням тегів
            var figuresWithTags = await context.Figures
                .Include(f => f.FigureTags)
                    .ThenInclude(ft => ft.Tag)
                .Where(f => f.FigureTags.Any())
                .ToListAsync();
            
            System.Console.WriteLine($"\n→ Фігур з тегами: {figuresWithTags.Count}");
            foreach (var fig in figuresWithTags)
            {
                var tagNames = string.Join(", ", fig.FigureTags.Select(ft => ft.Tag.Name));
                System.Console.WriteLine($"  • {fig.Name} → Теги: {tagNames}");
            }

            // Запит з метаданими
            var figuresWithMetadata = await context.Figures
                .Include(f => f.Metadata)
                .Where(f => f.Metadata != null)
                .ToListAsync();
            
            System.Console.WriteLine($"\n→ Фігур з метаданими: {figuresWithMetadata.Count}");
            foreach (var fig in figuresWithMetadata)
            {
                System.Console.WriteLine($"  • {fig.Name} → Автор: {fig.Metadata?.Author}");
            }
            System.Console.WriteLine();

            System.Console.WriteLine("9. ПІДСУМКОВА ІНФОРМАЦІЯ\n");

            var totalFigures = await context.Figures.CountAsync();
            var totalCollections = await context.Collections.CountAsync();
            var totalTags = await context.Tags.CountAsync();
            var totalMetadata = await context.FigureMetadata.CountAsync();

            System.Console.WriteLine($"→ Загальна статистика БД:");
            System.Console.WriteLine($"  • Всього фігур: {totalFigures}");
            System.Console.WriteLine($"  • Колекцій: {totalCollections}");
            System.Console.WriteLine($"  • Тегів: {totalTags}");
            System.Console.WriteLine($"  • Метаданих: {totalMetadata}");

            // ============================================================
            // MONGODB (NoSQL) ДЕМОНСТРАЦІЯ
            // ============================================================
            
            System.Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            System.Console.WriteLine("║                 ДОДАТКОВЕ ЗАВДАННЯ: MONGODB                    ║");
            System.Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            await DemonstrateMongoDbAsync();

            System.Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            System.Console.WriteLine("║                 ЛАБОРАТОРНА РОБОТА ЗАВЕРШЕНА                   ║");
            System.Console.WriteLine("║                                                                ║");
            System.Console.WriteLine("║  Для перегляду баз даних використайте:                        ║");
            System.Console.WriteLine("║  PostgreSQL:                                                   ║");
            System.Console.WriteLine("║  - pgAdmin (PostgreSQL GUI)                                    ║");
            System.Console.WriteLine("║  - DBeaver (Universal Database Tool)                           ║");
            System.Console.WriteLine("║  - psql -U postgres -d figuredb                                ║");
            System.Console.WriteLine("║                                                                ║");
            System.Console.WriteLine("║  MongoDB:                                                      ║");
            System.Console.WriteLine("║  - MongoDB Compass (MongoDB GUI)                               ║");
            System.Console.WriteLine("║  - mongosh (MongoDB Shell)                                     ║");
            System.Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        }

        static async Task DemonstrateMongoDbAsync()
        {
            try
            {
                // Підключення до MongoDB
                var mongoConnectionString = "mongodb://localhost:27017";
                var mongoClient = new MongoClient(mongoConnectionString);
                var database = mongoClient.GetDatabase("figuredb_nosql");

                System.Console.WriteLine($"→ Підключення до MongoDB: {mongoConnectionString}");
                
                // Перевірка підключення
                await database.ListCollectionNamesAsync();
                System.Console.WriteLine("  ✓ MongoDB підключено успішно\n");

                var mongoRepository = new MongoFigureRepository(database);

                System.Console.WriteLine("10. СТВОРЕННЯ ДОКУМЕНТІВ У MONGODB\n");

                // Створення фігур для MongoDB
                var mongoFigures = new List<Figure>
                {
                    Circle.CreateNew(),
                    Rectangle.CreateNew(),
                    Square.CreateNew(),
                    Triangle.CreateNew(),
                    Circle.CreateNew()
                };

                // Обчислення площі та периметру
                foreach (var fig in mongoFigures)
                {
                    fig.CalculateArea();
                    fig.CalculatePerimetr();
                }

                // Конвертація та збереження в MongoDB
                foreach (var fig in mongoFigures)
                {
                    var document = fig.ToDocument();
                    document.Tags = new List<string> { "новий", "тест" };
                    document.Metadata = new Dictionary<string, string>
                    {
                        { "author", "Роман Чорнорук" },
                        { "version", "1.0" }
                    };
                    
                    await mongoRepository.AddAsync(document);
                    System.Console.WriteLine($"  • Додано до MongoDB: {fig.Name} ({fig.GetType().Name})");
                }

                System.Console.WriteLine($"\n  ✓ Всього додано до MongoDB: {mongoFigures.Count} документів\n");

                System.Console.WriteLine("11. ЧИТАННЯ З MONGODB\n");

                var allDocuments = (await mongoRepository.GetAllAsync()).ToList();
                System.Console.WriteLine($"→ Знайдено документів: {allDocuments.Count}");
                
                foreach (var doc in allDocuments.Take(5))
                {
                    System.Console.WriteLine($"  • {doc.Name} - {doc.Type} (Площа: {doc.Area:F2})");
                    System.Console.WriteLine($"    Теги: {string.Join(", ", doc.Tags)}");
                }
                
                if (allDocuments.Count > 5)
                    System.Console.WriteLine($"  ... та ще {allDocuments.Count - 5} документів\n");
                else
                    System.Console.WriteLine();

                System.Console.WriteLine("12. ЗАПИТИ ДО MONGODB\n");

                // Запит за типом
                var circles = (await mongoRepository.GetByTypeAsync("Circle")).ToList();
                System.Console.WriteLine($"→ Кіл у MongoDB: {circles.Count}");

                // Запит за кольором
                var redFigures = (await mongoRepository.GetByColorAsync("червоний")).ToList();
                System.Console.WriteLine($"→ Червоних фігур: {redFigures.Count}");

                // Запит за тегом
                var testFigures = (await mongoRepository.GetByTagAsync("тест")).ToList();
                System.Console.WriteLine($"→ Фігур з тегом 'тест': {testFigures.Count}\n");

                System.Console.WriteLine("13. ОНОВЛЕННЯ ДОКУМЕНТА У MONGODB\n");

                var firstDoc = allDocuments.FirstOrDefault();
                if (firstDoc != null && firstDoc.Id != null)
                {
                    firstDoc.Name = "Оновлена фігура MongoDB";
                    firstDoc.Tags.Add("оновлено");
                    
                    await mongoRepository.UpdateAsync(firstDoc.Id, firstDoc);
                    System.Console.WriteLine($"  ✓ Оновлено документ: {firstDoc.Name}\n");
                }

                System.Console.WriteLine("14. ВИДАЛЕННЯ З MONGODB\n");

                var lastDoc = allDocuments.LastOrDefault();
                if (lastDoc != null && lastDoc.Id != null)
                {
                    await mongoRepository.DeleteAsync(lastDoc.Id);
                    System.Console.WriteLine($"  ✓ Видалено документ: {lastDoc.Name}\n");
                }

                // Фінальна статистика
                var finalCount = await mongoRepository.CountAsync();
                System.Console.WriteLine($"  Залишилось документів у MongoDB: {finalCount}\n");

                System.Console.WriteLine("15. ПОРІВНЯННЯ SQL vs NoSQL\n");
                System.Console.WriteLine("→ PostgreSQL (Реляційна БД):");
                System.Console.WriteLine("  + Строга схема даних");
                System.Console.WriteLine("  + ACID транзакції");
                System.Console.WriteLine("  + Складні JOIN запити");
                System.Console.WriteLine("  + Нормалізація даних");
                
                System.Console.WriteLine("\n→ MongoDB (Документо-орієнтована БД):");
                System.Console.WriteLine("  + Гнучка схема (schema-less)");
                System.Console.WriteLine("  + Швидке читання/запис");
                System.Console.WriteLine("  + Вбудовані масиви та об'єкти");
                System.Console.WriteLine("  + Горизонтальне масштабування\n");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"\n  ✗ Помилка роботи з MongoDB: {ex.Message}");
                System.Console.WriteLine("  Переконайтеся, що MongoDB запущено локально на порту 27017");
                System.Console.WriteLine("  Запустити MongoDB: mongod --dbpath <path_to_data>");
            }
        }
    }
}
