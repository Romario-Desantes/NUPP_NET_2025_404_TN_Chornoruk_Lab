# Лабораторна робота №4 - Підсумок

**Виконав:** Чорнорук Роман  
**Група:** 404-TN  
**Дата:** 23 грудня 2025

## ✅ Виконані завдання

### 1. Створено проєкт ASP.NET Core Web API
- ✅ Проєкт `FigureProj.REST` створено за допомогою команди `dotnet new webapi`
- ✅ Додано до solution файлу
- ✅ Додано посилання на проєкти `FigureProj.Common` та `FigureProj.Infrastructure`

### 2. Створено папку Models з DTO моделями
- ✅ `FigureDto` - для читання фігур
- ✅ `CreateFigureDto` - для створення фігур (з валідацією)
- ✅ `UpdateFigureDto` - для оновлення фігур
- ✅ `CollectionDto` - для читання колекцій
- ✅ `CreateCollectionDto` - для створення колекцій
- ✅ `UpdateCollectionDto` - для оновлення колекцій

### 3. Створено контролери
- ✅ `FiguresController` - CRUD операції для фігур
  - GET /api/figures - отримати всі
  - GET /api/figures/{id} - отримати за ID
  - GET /api/figures/page - пагінація
  - POST /api/figures - створити
  - PUT /api/figures/{id} - оновити
  - DELETE /api/figures/{id} - видалити

- ✅ `CollectionsController` - CRUD операції для колекцій
  - GET /api/collections - отримати всі
  - GET /api/collections/{id} - отримати за ID
  - GET /api/collections/{id}/figures - отримати фігури в колекції
  - POST /api/collections - створити
  - PUT /api/collections/{id} - оновити
  - DELETE /api/collections/{id} - видалити

### 4. Дотримання REST принципів
- ✅ Унікальні URI для ресурсів
- ✅ Правильні HTTP методи (GET, POST, PUT, DELETE)
- ✅ Правильні HTTP коди відповідей:
  - 200 OK - успішне читання/оновлення
  - 201 Created - успішне створення
  - 204 No Content - успішне видалення
  - 400 Bad Request - помилка валідації
  - 404 Not Found - ресурс не знайдено

### 5. Використання асинхронного CRUD сервісу
- ✅ Використовується `ICrudServiceAsync<T>` з 3-ї лабораторної
- ✅ Всі операції асинхронні (async/await)
- ✅ Використовується `DbCrudServiceAsync<Figure>`

### 6. Dependency Injection
- ✅ Налаштовано в `Program.cs`
- ✅ Зареєстровано `FigureContext`
- ✅ Зареєстровано `IRepository<FigureModel>`
- ✅ Зареєстровано `ICrudServiceAsync<Figure>`
- ✅ Впроваджено в контролери через конструктор

### 7. Тестування API
- ✅ Створено PowerShell скрипт `test_api.ps1`
- ✅ Протестовано всі endpoints
- ✅ Всі тести пройшли успішно
- ✅ Swagger UI доступний на http://localhost:5000

### 8. Документація
- ✅ Створено `LAB4_DOCUMENTATION.md` з повною документацією
- ✅ Створено `README.md` для проєкту REST API
- ✅ Відповіді на всі контрольні запитання
- ✅ Приклади запитів та відповідей

## 📊 Результати тестування

Всі 13 тестів пройшли успішно:

1. ✅ Створення кола (POST)
2. ✅ Створення прямокутника (POST)
3. ✅ Створення квадрата (POST)
4. ✅ Створення трикутника (POST)
5. ✅ Отримання всіх фігур (GET)
6. ✅ Отримання фігури за ID (GET)
7. ✅ Оновлення фігури (PUT)
8. ✅ Пагінація (GET)
9. ✅ Створення колекції (POST)
10. ✅ Отримання всіх колекцій (GET)
11. ✅ Оновлення колекції (PUT)
12. ✅ Видалення фігури (DELETE)
13. ✅ Перевірка видалення - 404 (GET)

## 📁 Структура проєкту

```
FigureProj.REST/
├── Controllers/
│   ├── FiguresController.cs
│   └── CollectionsController.cs
├── Models/
│   ├── FigureDto.cs
│   ├── CreateFigureDto.cs
│   ├── UpdateFigureDto.cs
│   ├── CollectionDto.cs
│   ├── CreateCollectionDto.cs
│   └── UpdateCollectionDto.cs
├── Program.cs
├── appsettings.json
└── README.md
```

## 🔧 Технології

- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL
- Swagger/OpenAPI
- Dependency Injection
- Асинхронне програмування

## 📝 Git

- Створено гілку `lab4`
- Зроблено commit з усіма змінами
- Готово до створення Pull Request

## 🎯 Наступні кроки

Для здачі лабораторної роботи:

1. Створити Pull Request з гілки `lab4` в `master` (або в `lab3` якщо PR з lab3 ще не закритий)
2. Додати PDF файл з результатами тестування (можна використати LAB4_DOCUMENTATION.md)
3. Переконатися, що всі тести проходять

## 📖 Відповіді на контрольні запитання

Всі відповіді детально описані в `LAB4_DOCUMENTATION.md`:

1. ✅ Ключові особливості ASP.NET Core Web API
2. ✅ 3-рівнева архітектура та її переваги
3. ✅ Навіщо потрібні окремі DTO моделі
4. ✅ REST та принцип однорідності інтерфейсу
5. ✅ HTTP методи для CRUD операцій

---

**Статус:** ✅ ЗАВЕРШЕНО  
**Оцінка:** Готово до здачі

