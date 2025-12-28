using FigureProj.Common.Models.Abstract;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace FigureProj.Common.Services
{
    public class CrudService<T> : ICrudService<T> where T : Figure
    {
        // Колекція для зберігання даних
        private readonly Dictionary<Guid, T> _storage;

        // Конструктор
        public CrudService()
        {
            _storage = new Dictionary<Guid, T>();
        }

        // Метод для створення (додавання) елемента
        public void Create(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (_storage.ContainsKey(element.Id))
                throw new InvalidOperationException($"Елемент з ID {element.Id} вже існує.");

            _storage.Add(element.Id, element);
            Console.WriteLine($"Створено елемент з ID: {element.Id}");
        }

        // Метод для читання елемента за ID
        public T Read(Guid id)
        {
            if (!_storage.ContainsKey(id))
                throw new KeyNotFoundException($"Елемент з ID {id} не знайдено.");

            return _storage[id];
        }

        // Метод для читання всіх елементів
        public IEnumerable<T> ReadAll()
        {
            return _storage.Values;
        }

        // Метод для оновлення елемента
        public void Update(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!_storage.ContainsKey(element.Id))
                throw new KeyNotFoundException($"Елемент з ID {element.Id} не знайдено для оновлення.");

            _storage[element.Id] = element;
            Console.WriteLine($"Оновлено елемент з ID: {element.Id}");
        }

        // Метод для видалення елемента
        public void Remove(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            if (!_storage.ContainsKey(element.Id))
                throw new KeyNotFoundException($"Елемент з ID {element.Id} не знайдено для видалення.");

            _storage.Remove(element.Id);
            Console.WriteLine($"Видалено елемент з ID: {element.Id}");
        }

        // Статичний метод для отримання кількості елементів
        public static int GetCount(CrudService<T> service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            return service._storage.Count;
        }

        // Метод для збереження даних у файл
        public void Save(string FilePath)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(FilePath));

            try
            {
                // Отримуємо всі фігури зі сховища як List<Figure> для підтримки поліморфізму
                var figures = _storage.Values.Cast<Figure>().ToList();

                // Налаштування для JSON серіалізації з підтримкою поліморфізму
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    IncludeFields = true,
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                };

                // Серіалізуємо дані у JSON
                string json = JsonSerializer.Serialize(figures, options);

                // Записуємо JSON у файл
                File.WriteAllText(FilePath, json);

                Console.WriteLine($"Дані успішно збережено у файл: {FilePath}");
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new DirectoryNotFoundException($"Директорія для файлу не знайдена: {ex.Message}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Немає доступу до запису у файл: {ex.Message}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Помилка запису у файл: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка при збереженні даних: {ex.Message}", ex);
            }
        }

        // Метод для завантаження даних з файлу
        public void Load(string FilePath)
        {
            if (string.IsNullOrWhiteSpace(FilePath))
                throw new ArgumentException("Шлях до файлу не може бути порожнім.", nameof(FilePath));

            if (!File.Exists(FilePath))
                throw new FileNotFoundException($"Файл не знайдено: {FilePath}");

            try
            {
                // Читаємо JSON з файлу
                string json = File.ReadAllText(FilePath);

                if (string.IsNullOrWhiteSpace(json))
                {
                    Console.WriteLine("Файл порожній. Сховище залишиться незмінним.");
                    return;
                }

                // Налаштування для JSON десеріалізації з підтримкою поліморфізму
                var options = new JsonSerializerOptions
                {
                    IncludeFields = true,
                    TypeInfoResolver = new DefaultJsonTypeInfoResolver()
                };

                // Десеріалізуємо дані з JSON (використовуємо List<Figure> для підтримки поліморфізму)
                var figures = JsonSerializer.Deserialize<List<Figure>>(json, options);

                if (figures == null)
                {
                    throw new InvalidOperationException("Не вдалося десеріалізувати дані з файлу.");
                }

                // Очищаємо поточне сховище
                _storage.Clear();

                // Заповнюємо сховище завантаженими даними
                foreach (var figure in figures)
                {
                    if (figure != null && figure is T typedFigure)
                    {
                        _storage[figure.Id] = typedFigure;
                    }
                }

                Console.WriteLine($"Дані успішно завантажено з файлу: {FilePath}. Завантажено {_storage.Count} елементів.");
            }
            catch (JsonException ex)
            {
                throw new JsonException($"Помилка формату JSON у файлі: {ex.Message}", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new UnauthorizedAccessException($"Немає доступу до читання файлу: {ex.Message}", ex);
            }
            catch (IOException ex)
            {
                throw new IOException($"Помилка читання файлу: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Помилка при завантаженні даних: {ex.Message}", ex);
            }
        }
    }
}

