using System.Collections;
using System.Collections.Concurrent;
using System.Text.Json;
using FigureProj.Common.Models.Abstract;

namespace FigureProj.Common.Services
{
    public class CrudServiceAsync<T> : ICrudServiceAsync<T> where T : Figure
    {
        // Thread-safe колекція для зберігання даних
        private readonly ConcurrentDictionary<Guid, T> _storage;
        
        // Семафор для асинхронних операцій з файлами
        private readonly SemaphoreSlim _fileSemaphore;
        
        // Шлях до файлу для збереження
        public string FilePath { get; set; }

        // Конструктор
        public CrudServiceAsync(string filePath = "data.json")
        {
            _storage = new ConcurrentDictionary<Guid, T>();
            _fileSemaphore = new SemaphoreSlim(1, 1);
            FilePath = filePath;
        }

        // Метод для асинхронного створення елемента
        public async Task<bool> CreateAsync(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            await Task.Run(() =>
            {
                if (!_storage.TryAdd(element.Id, element))
                    throw new InvalidOperationException($"Елемент з ID {element.Id} вже існує.");
            });

            return true;
        }

        // Метод для асинхронного читання елемента за ID
        public async Task<T> ReadAsync(Guid id)
        {
            return await Task.Run(() =>
            {
                if (!_storage.TryGetValue(id, out T? element))
                    throw new KeyNotFoundException($"Елемент з ID {id} не знайдено.");

                return element;
            });
        }

        // Метод для асинхронного читання всіх елементів
        public async Task<IEnumerable<T>> ReadAllAsync()
        {
            return await Task.Run(() => _storage.Values.AsEnumerable());
        }

        // Метод для асинхронного читання з пагінацією
        public async Task<IEnumerable<T>> ReadAllAsync(int page, int amount)
        {
            if (page <= 0)
                throw new ArgumentOutOfRangeException(nameof(page), "Номер сторінки повинен бути більше 0.");

            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), "Кількість елементів повинна бути більше 0.");

            return await Task.Run(() =>
            {
                return _storage.Values
                    .Skip((page - 1) * amount)
                    .Take(amount)
                    .ToList();
            });
        }

        // Метод для асинхронного оновлення елемента
        public async Task<bool> UpdateAsync(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return await Task.Run(() =>
            {
                if (!_storage.ContainsKey(element.Id))
                    throw new KeyNotFoundException($"Елемент з ID {element.Id} не знайдено для оновлення.");

                _storage[element.Id] = element;
                return true;
            });
        }

        // Метод для асинхронного видалення елемента
        public async Task<bool> RemoveAsync(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            return await Task.Run(() =>
            {
                if (!_storage.TryRemove(element.Id, out _))
                    throw new KeyNotFoundException($"Елемент з ID {element.Id} не знайдено для видалення.");

                return true;
            });
        }

        // Метод для асинхронного збереження колекції у файл
        public async Task<bool> SaveAsync()
        {
            await _fileSemaphore.WaitAsync();
            try
            {
                var data = _storage.Values.ToList();
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true 
                };

                using var stream = File.Create(FilePath);
                await JsonSerializer.SerializeAsync(stream, data, options);
                
                return true;
            }
            finally
            {
                _fileSemaphore.Release();
            }
        }

        // Реалізація IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            return _storage.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Додатковий метод для отримання кількості елементів
        public int Count => _storage.Count;
    }
}

