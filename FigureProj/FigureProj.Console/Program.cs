using FigureProj.Common.Models;
using FigureProj.Common.Models.Abstract;
using FigureProj.Common.Services;
using FigureProj.Common.Extensions;

namespace FigureProj.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.OutputEncoding = System.Text.Encoding.UTF8;
            System.Console.WriteLine("ДЕМОНСТРАЦІЯ CRUD СЕРВІСУ ДЛЯ ГЕОМЕТРИЧНИХ ФІГУР");

            // Створення екземпляру CRUD сервісу
            var figureService = new CrudService<Figure>();

            // Обробник події для фігур
            void FigurePropertyChangedHandler(Figure figure, string message)
            {
                System.Console.WriteLine($"Подія: {message} для фігури '{figure.Name}'");
            }

            System.Console.WriteLine("\n1. СТВОРЕННЯ ФІГУР (CREATE)\n");

            // Створення фігур різних типів
            var circle = new Circle(5.5, "Коло 1", "синій");
            circle.OnPropertyChanged += FigurePropertyChangedHandler;
            figureService.Create(circle);

            var rectangle = new Rectangle(4.0, 6.0, "Прямокутник 1", "червоний");
            rectangle.OnPropertyChanged += FigurePropertyChangedHandler;
            figureService.Create(rectangle);

            var triangle = new Triangle(3.0, 4.0, 5.0, "Трикутник 1", "жовтий");
            triangle.OnPropertyChanged += FigurePropertyChangedHandler;
            figureService.Create(triangle);

            var square = new Square(7.0, "Квадрат 1", "фіолетовий");
            square.OnPropertyChanged += FigurePropertyChangedHandler;
            figureService.Create(square);

            System.Console.WriteLine($"\nВсього фігур у сервісі: {CrudService<Figure>.GetCount(figureService)}");

            System.Console.WriteLine("\n2. ЧИТАННЯ ВСІХ ФІГУР (READ ALL)\n");

            var allFigures = figureService.ReadAll();
            foreach (var figure in allFigures)
            {
                System.Console.WriteLine(figure.ToString());
            }

            System.Console.WriteLine("\n3. ЧИТАННЯ КОНКРЕТНОЇ ФІГУРИ (READ)\n");

            var readCircle = figureService.Read(circle.Id);
            System.Console.WriteLine($"Прочитано фігуру: {readCircle.ToString()}");

            System.Console.WriteLine("\n4. ДЕМОНСТРАЦІЯ МЕТОДУ РОЗШИРЕННЯ\n");

            // Використання методу розширення для детальної інформації
            System.Console.WriteLine(circle.GetDetailedInfo());
            System.Console.WriteLine(square.GetDetailedInfo());

            System.Console.WriteLine("\n5. ОНОВЛЕННЯ ФІГУРИ (UPDATE)\n");

            System.Console.WriteLine("Змінюємо радіус кола...");
            circle.Radius = 10.0; // Це викличе подію
            figureService.Update(circle);
            System.Console.WriteLine($"Оновлена фігура: {circle.ToString()}");

            System.Console.WriteLine("\n6. ДЕМОНСТРАЦІЯ СТАТИЧНИХ ПОЛІВ\n");

            System.Console.WriteLine($"Загальна кількість створених фігур (Counter): {Figure.CounterOfFigures}");
            System.Console.WriteLine($"Колір за замовчуванням: {Figure.DefaultColor}");
            System.Console.WriteLine("\nДоступні кольори:");
            foreach (var colorPair in Figure.PresetColors)
            {
                System.Console.WriteLine($"  • {colorPair.Key} → {colorPair.Value}");
            }

            System.Console.WriteLine("\n7. ВИДАЛЕННЯ ФІГУРИ (REMOVE)\n");

            figureService.Remove(triangle);
            System.Console.WriteLine($"Фігур після видалення: {CrudService<Figure>.GetCount(figureService)}");

            System.Console.WriteLine("\n8. ПЕРЕВІРКА ПІСЛЯ ВИДАЛЕННЯ\n");

            System.Console.WriteLine("Фігури, що залишилися:");
            foreach (var figure in figureService.ReadAll())
            {
                System.Console.WriteLine($"  • {figure.Name} (ID: {figure.Id})");
            }

            System.Console.WriteLine("\n9. МАЛЮВАННЯ ФІГУР\n");

            System.Console.WriteLine($"Малюємо {circle.Name}:");
            circle.Draw();

            System.Console.WriteLine($"\nМалюємо {rectangle.Name}:");
            rectangle.Draw();

            System.Console.WriteLine($"\nМалюємо {square.Name}:");
            square.Draw();

            System.Console.WriteLine("\n10. ПІДСУМКОВА СТАТИСТИКА\n");

            System.Console.WriteLine($"Всього фігур у системі: {Figure.CounterOfFigures}");
            System.Console.WriteLine($"Фігур у сервісі: {CrudService<Figure>.GetCount(figureService)}");

            System.Console.WriteLine("\nДетальна інформація про всі фігури в сервісі:");
            foreach (var figure in figureService.ReadAll())
            {
                System.Console.WriteLine(figure.GetDetailedInfo());
            }

            System.Console.WriteLine("\n11. ЗБЕРЕЖЕННЯ ДАНИХ У ФАЙЛ (SAVE)\n");

            string filePath = "figures_data.json";
            try
            {
                figureService.Save(filePath);
                System.Console.WriteLine($"Файл збережено: {System.IO.Path.GetFullPath(filePath)}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Помилка при збереженні: {ex.Message}");
            }

            System.Console.WriteLine("\n12. ЗАВАНТАЖЕННЯ ДАНИХ З ФАЙЛУ (LOAD)\n");

            // Створюємо новий сервіс для демонстрації завантаження
            var newFigureService = new CrudService<Figure>();
            System.Console.WriteLine($"Кількість фігур у новому сервісі до завантаження: {CrudService<Figure>.GetCount(newFigureService)}");

            try
            {
                newFigureService.Load(filePath);
                System.Console.WriteLine($"\nФігури після завантаження з файлу:");
                foreach (var figure in newFigureService.ReadAll())
                {
                    System.Console.WriteLine($"  • {figure.Name} (ID: {figure.Id}, Тип: {figure.GetType().Name})");
                }
                System.Console.WriteLine($"\nВсього завантажено фігур: {CrudService<Figure>.GetCount(newFigureService)}");
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Помилка при завантаженні: {ex.Message}");
            }
        }
    }
}
