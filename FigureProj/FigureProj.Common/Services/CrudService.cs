using FigureProj.Common.Models.Abstract;

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
    }
}

