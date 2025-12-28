# FigureProj.REST - ASP.NET Core Web API

REST API для роботи з геометричними фігурами (Лабораторна робота №4).

## Запуск проєкту

### Вимоги
- .NET 8.0 SDK
- PostgreSQL (запущений на localhost:5432)
- База даних: figuredb

### Запуск

```bash
cd FigureProj.REST
dotnet run --urls "http://localhost:5000"
```

Swagger UI буде доступний за адресою: http://localhost:5000

## Endpoints

### Фігури (Figures)

- `GET /api/figures` - Отримати всі фігури
- `GET /api/figures/{id}` - Отримати фігуру за ID
- `GET /api/figures/page?page=1&pageSize=10` - Пагінація
- `POST /api/figures` - Створити нову фігуру
- `PUT /api/figures/{id}` - Оновити фігуру
- `DELETE /api/figures/{id}` - Видалити фігуру

### Колекції (Collections)

- `GET /api/collections` - Отримати всі колекції
- `GET /api/collections/{id}` - Отримати колекцію за ID
- `GET /api/collections/{id}/figures` - Отримати фігури в колекції
- `POST /api/collections` - Створити нову колекцію
- `PUT /api/collections/{id}` - Оновити колекцію
- `DELETE /api/collections/{id}` - Видалити колекцію

## Приклади запитів

### Створення кола

```bash
POST /api/figures
Content-Type: application/json

{
  "name": "Test Circle",
  "color": "червоний",
  "type": "Circle",
  "radius": 15.5
}
```

### Створення прямокутника

```bash
POST /api/figures
Content-Type: application/json

{
  "name": "Test Rectangle",
  "color": "синій",
  "type": "Rectangle",
  "width": 20.0,
  "height": 10.0
}
```

## Тестування

Запустити автоматичні тести:

```powershell
.\test_api.ps1
```

## Архітектура

Проєкт використовує 3-рівневу архітектуру:

1. **Presentation Layer** - Controllers, DTO Models
2. **Business Logic Layer** - Domain Models, Services
3. **Data Access Layer** - Database Models, Repositories

## Технології

- ASP.NET Core 8.0
- Entity Framework Core
- PostgreSQL
- Swagger/OpenAPI
- Dependency Injection

