using FigureProj.Common.Models;
using FigureProj.Common.Models.Abstract;
using FigureProj.Common.Services;
using FigureProj.Common.Extensions;

namespace FigureProj.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            System.Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
            System.Console.WriteLine("║  ЛАБОРАТОРНА РОБОТА №2: АСИНХРОННИЙ CRUD СЕРВІС З БАГАТОПОТОКОВІСТЮ  ║");
            System.Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

            // Створення екземпляру асинхронного CRUD сервісу
            var figureService = new CrudServiceAsync<Figure>("figures_data.json");

            System.Console.WriteLine("1. ПАРАЛЕЛЬНЕ СТВОРЕННЯ 1000+ ФІГУР\n");

            // Лічильник для демонстрації thread-safety
            int createdCount = 0;
            object lockObject = new object();

            // Паралельне створення фігур
            var figures = new List<Figure>();
            
            Parallel.For(0, 1000, i =>
            {
                Figure figure;
                
                // Створюємо різні типи фігур
                switch (i % 4)
                {
                    case 0:
                        figure = Circle.CreateNew();
                        break;
                    case 1:
                        figure = Rectangle.CreateNew();
                        break;
                    case 2:
                        figure = Square.CreateNew();
                        break;
                    default:
                        figure = Triangle.CreateNew();
                        break;
                }

                // Thread-safe додавання до списку
                lock (lockObject)
                {
                    figures.Add(figure);
                    createdCount++;
                }
            });

            System.Console.WriteLine($"Створено фігур паралельно: {createdCount}");

            System.Console.WriteLine("\n2. ДОДАВАННЯ ФІГУР ДО АСИНХРОННОГО CRUD СЕРВІСУ\n");

            // Асинхронне додавання всіх фігур
            var tasks = figures.Select(f => figureService.CreateAsync(f)).ToList();
            await Task.WhenAll(tasks);

            System.Console.WriteLine($"Додано до сервісу: {figureService.Count} фігур");

            System.Console.WriteLine("\n3. ДЕМОНСТРАЦІЯ ПРИМІТИВІВ СИНХРОНІЗАЦІЇ\n");

            // Демонстрація Lock
            System.Console.WriteLine("→ Приклад LOCK:");
            int sharedCounter = 0;
            var lockDemo = new object();
            
            Parallel.For(0, 100, i =>
            {
                lock (lockDemo)
                {
                    sharedCounter++;
                }
            });
            System.Console.WriteLine($"  Лічильник після 100 паралельних інкрементів: {sharedCounter}");

            // Демонстрація Semaphore
            System.Console.WriteLine("\n→ Приклад SEMAPHORE (обмеження до 3 одночасних потоків):");
            var semaphore = new SemaphoreSlim(3, 3);
            var semaphoreTasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                int taskNumber = i;
                semaphoreTasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        System.Console.WriteLine($"  Потік {taskNumber} виконується...");
                        await Task.Delay(100);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }
            await Task.WhenAll(semaphoreTasks);
            System.Console.WriteLine("  Всі потоки завершено");

            // Демонстрація AutoResetEvent
            System.Console.WriteLine("\n→ Приклад AUTORESETEVENT:");
            var autoEvent = new AutoResetEvent(false);
            bool eventReceived = false;

            var eventTask = Task.Run(() =>
            {
                System.Console.WriteLine("  Потік очікує на сигнал...");
                autoEvent.WaitOne();
                eventReceived = true;
                System.Console.WriteLine("  Сигнал отримано!");
            });

            await Task.Delay(200);
            System.Console.WriteLine("  Відправка сигналу...");
            autoEvent.Set();
            await eventTask;

            // Демонстрація Monitor
            System.Console.WriteLine("\n→ Приклад MONITOR:");
            var monitorLock = new object();
            int monitorCounter = 0;

            Parallel.For(0, 50, i =>
            {
                Monitor.Enter(monitorLock);
                try
                {
                    monitorCounter++;
                }
                finally
                {
                    Monitor.Exit(monitorLock);
                }
            });
            System.Console.WriteLine($"  Лічильник Monitor після 50 операцій: {monitorCounter}");

            System.Console.WriteLine("\n4. СТАТИСТИКА З ВИКОРИСТАННЯМ LINQ\n");

            // Обчислення площ для всіх фігур
            foreach (var figure in figures)
            {
                figure.CalculateArea();
                figure.CalculatePerimetr();
            }

            // Загальна статистика
            var areas = figures.Select(f => f.Area).ToList();
            var perimeters = figures.Select(f => f.Perimeter).ToList();

            System.Console.WriteLine("→ Загальна статистика для всіх фігур:");
            System.Console.WriteLine($"  Мінімальна площа:    {areas.Min():F2}");
            System.Console.WriteLine($"  Максимальна площа:   {areas.Max():F2}");
            System.Console.WriteLine($"  Середня площа:       {areas.Average():F2}");
            System.Console.WriteLine($"  Мінімальний периметр: {perimeters.Min():F2}");
            System.Console.WriteLine($"  Максимальний периметр: {perimeters.Max():F2}");
            System.Console.WriteLine($"  Середній периметр:    {perimeters.Average():F2}");

            // Статистика по кожному типу фігур
            System.Console.WriteLine("\n→ Статистика для Кіл:");
            var circles = figures.OfType<Circle>().ToList();
            if (circles.Any())
            {
                var radii = circles.Select(c => c.Radius).ToList();
                System.Console.WriteLine($"  Кількість:           {circles.Count}");
                System.Console.WriteLine($"  Мінімальний радіус:  {radii.Min():F2}");
                System.Console.WriteLine($"  Максимальний радіус: {radii.Max():F2}");
                System.Console.WriteLine($"  Середній радіус:     {radii.Average():F2}");
                System.Console.WriteLine($"  Середня площа:       {circles.Select(c => c.Area).Average():F2}");
            }

            System.Console.WriteLine("\n→ Статистика для Прямокутників:");
            var rectangles = figures.OfType<Rectangle>().ToList();
            if (rectangles.Any())
            {
                var widths = rectangles.Select(r => r.Width).ToList();
                var heights = rectangles.Select(r => r.Height).ToList();
                System.Console.WriteLine($"  Кількість:           {rectangles.Count}");
                System.Console.WriteLine($"  Середня ширина:      {widths.Average():F2}");
                System.Console.WriteLine($"  Середня висота:      {heights.Average():F2}");
                System.Console.WriteLine($"  Середня площа:       {rectangles.Select(r => r.Area).Average():F2}");
            }

            System.Console.WriteLine("\n→ Статистика для Квадратів:");
            var squares = figures.OfType<Square>().ToList();
            if (squares.Any())
            {
                var sides = squares.Select(s => s.Side).ToList();
                System.Console.WriteLine($"  Кількість:           {squares.Count}");
                System.Console.WriteLine($"  Мінімальна сторона:  {sides.Min():F2}");
                System.Console.WriteLine($"  Максимальна сторона: {sides.Max():F2}");
                System.Console.WriteLine($"  Середня сторона:     {sides.Average():F2}");
                System.Console.WriteLine($"  Середня площа:       {squares.Select(s => s.Area).Average():F2}");
            }

            System.Console.WriteLine("\n→ Статистика для Трикутників:");
            var triangles = figures.OfType<Triangle>().ToList();
            if (triangles.Any())
            {
                System.Console.WriteLine($"  Кількість:           {triangles.Count}");
                System.Console.WriteLine($"  Середня сторона A:   {triangles.Select(t => t.A).Average():F2}");
                System.Console.WriteLine($"  Середня сторона B:   {triangles.Select(t => t.B).Average():F2}");
                System.Console.WriteLine($"  Середня сторона C:   {triangles.Select(t => t.C).Average():F2}");
                System.Console.WriteLine($"  Середня площа:       {triangles.Select(t => t.Area).Average():F2}");
            }

            System.Console.WriteLine("\n5. ДЕМОНСТРАЦІЯ ПАГІНАЦІЇ\n");

            int pageSize = 10;
            System.Console.WriteLine($"→ Перша сторінка (page=1, amount={pageSize}):");
            var page1 = await figureService.ReadAllAsync(1, pageSize);
            foreach (var figure in page1.Take(5))
            {
                System.Console.WriteLine($"  • {figure.Name} ({figure.GetType().Name}) - ID: {figure.Id.ToString().Substring(0, 8)}...");
            }
            System.Console.WriteLine($"  ... та ще {page1.Count() - 5} фігур");

            System.Console.WriteLine($"\n→ П'ята сторінка (page=5, amount={pageSize}):");
            var page5 = await figureService.ReadAllAsync(5, pageSize);
            foreach (var figure in page5.Take(5))
            {
                System.Console.WriteLine($"  • {figure.Name} ({figure.GetType().Name}) - ID: {figure.Id.ToString().Substring(0, 8)}...");
            }

            System.Console.WriteLine("\n6. ЗБЕРЕЖЕННЯ КОЛЕКЦІЇ У ФАЙЛ\n");

            System.Console.WriteLine("→ Асинхронне збереження у JSON файл...");
            bool saved = await figureService.SaveAsync();
            
            if (saved)
            {
                var fileInfo = new FileInfo(figureService.FilePath);
                System.Console.WriteLine($"  ✓ Дані успішно збережено у файл: {figureService.FilePath}");
                System.Console.WriteLine($"  Розмір файлу: {fileInfo.Length / 1024.0:F2} KB");
            }

            System.Console.WriteLine("\n7. ВИКОРИСТАННЯ IENUMERABLE\n");

            System.Console.WriteLine("→ Перебір колекції через IEnumerable:");
            int count = 0;
            foreach (var figure in figureService)
            {
                count++;
                if (count <= 5)
                {
                    System.Console.WriteLine($"  • {figure.Name}");
                }
            }
            System.Console.WriteLine($"  ... всього {count} фігур у колекції");

            System.Console.WriteLine("\n8. СТАТИСТИКА ПО КОЛЬОРАМ (LINQ)\n");

            var colorStats = figures
                .GroupBy(f => f.Color)
                .Select(g => new { Color = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            System.Console.WriteLine("→ Розподіл фігур по кольорам:");
            foreach (var stat in colorStats)
            {
                System.Console.WriteLine($"  {stat.Color,-15}: {stat.Count,4} фігур");
            }

            System.Console.WriteLine("\n9. ТОП-10 НАЙБІЛЬШИХ ФІГУР ЗА ПЛОЩЕЮ\n");

            var top10 = figures
                .OrderByDescending(f => f.Area)
                .Take(10)
                .ToList();

            System.Console.WriteLine("→ Фігури з найбільшою площею:");
            for (int i = 0; i < top10.Count; i++)
            {
                var figure = top10[i];
                System.Console.WriteLine($"  {i + 1,2}. {figure.Name,-20} - Площа: {figure.Area,8:F2} ({figure.GetType().Name})");
            }

            System.Console.WriteLine("\n10. ПІДСУМКОВА ІНФОРМАЦІЯ\n");

            System.Console.WriteLine($"→ Загальна кількість створених фігур: {Figure.CounterOfFigures}");
            System.Console.WriteLine($"→ Фігур у CRUD сервісі: {figureService.Count}");
            System.Console.WriteLine($"→ Використано типів фігур: {figures.GroupBy(f => f.GetType()).Count()}");
            System.Console.WriteLine($"→ Використано кольорів: {colorStats.Count}");

            System.Console.WriteLine("\n╔════════════════════════════════════════════════════════════════╗");
            System.Console.WriteLine("║                 ЛАБОРАТОРНА РОБОТА ЗАВЕРШЕНА                   ║");
            System.Console.WriteLine("╚════════════════════════════════════════════════════════════════╝");
        }
    }
}
