# Лабораторна робота №3: Робота з базами даних в .NET

## Опис проекту

Проект демонструє роботу з реляційними (PostgreSQL) та нереляційними (MongoDB) базами даних у .NET 8 з використанням Entity Framework Core та MongoDB Driver.

## Структура проекту

```
FigureProj/
├── FigureProj.Common/          # Доменні моделі (Figure, Circle, Rectangle, Square, Triangle)
├── FigureProj.Infrastructure/  # EF Core, PostgreSQL, Repository pattern
│   ├── Models/                 # Моделі для бази даних
│   ├── Repositories/           # Реалізація Repository pattern
│   ├── Services/               # DbCrudServiceAsync
│   ├── Mapping/                # Конвертація Domain ↔ Database
│   ├── Migrations/             # EF Core міграції
│   └── FigureContext.cs        # DbContext для PostgreSQL
├── FigureProj.NoSql/           # MongoDB репозиторії
│   ├── Models/                 # POCO класи для MongoDB
│   ├── Repositories/           # MongoDB Repository
│   └── Mapping/                # Конвертація Domain ↔ MongoDB
├── FigureProj.Console/         # Консольний застосунок
└── FigureProj.Tests/           # Юніт-тести
```

## Реалізовані зв'язки

### PostgreSQL (Реляційна БД)

#### Table-per-Type (TPT) для ієрархії фігур:
- Базова таблиця `Figures`
- Окремі таблиці для кожного типу: `Circles`, `Rectangles`, `Squares`, `Triangles`

#### Один-до-одного:
- `Figures` ↔ `FigureMetadata`
- Кожна фігура має одну запис метаданих (Author, Description, CreatedBy)

#### Один-до-багатьох:
- `Collections` → `Figures`
- Одна колекція може містити багато фігур

#### Багато-до-багатьох:
- `Figures` ↔ `Tags` (через проміжну таблицю `FigureTags`)
- Багато фігур можуть мати багато тегів

### MongoDB (NoSQL)

- Документо-орієнтована модель
- Гнучка схема (schema-less)
- Вбудовані масиви (Tags) та об'єкти (Metadata)

## Технології

- **.NET 8.0**
- **Entity Framework Core 8.0.11**
- **PostgreSQL** (через Npgsql.EntityFrameworkCore.PostgreSQL 8.0.11)
- **MongoDB Driver 2.29.0**
- **Repository Pattern**
- **Fluent API** для конфігурації EF Core

## Встановлення та запуск

### Передумови

1. **.NET 8 SDK**
2. **PostgreSQL** (версія 12 або вище)
3. **MongoDB** (версія 4.4 або вище) - для додаткового завдання

### Крок 1: Клонування репозиторію

```bash
git clone <repository_url>
cd FigureProj
git checkout lab3
```

### Крок 2: Налаштування PostgreSQL

1. Встановіть PostgreSQL
2. Створіть базу даних (опціонально, створюється автоматично):
```sql
CREATE DATABASE figuredb;
```

3. Оновіть connection string в `FigureProj.Console/Program.cs`:
```csharp
var connectionString = "Host=localhost;Database=figuredb;Username=postgres;Password=ВАШ_ПАРОЛЬ";
```

Або в `FigureProj.Infrastructure/FigureContextFactory.cs` для міграцій:
```csharp
optionsBuilder.UseNpgsql("Host=localhost;Database=figuredb;Username=postgres;Password=ВАШ_ПАРОЛЬ");
```

### Крок 3: Застосування міграцій

```bash
cd FigureProj.Infrastructure
dotnet ef database update
```

Або міграції застосуються автоматично при запуску програми.

### Крок 4: Налаштування MongoDB (опціонально)

1. Встановіть MongoDB
2. Запустіть MongoDB сервер:
```bash
mongod --dbpath /path/to/data
```

Connection string за замовчуванням: `mongodb://localhost:27017`

### Крок 5: Запуск програми

```bash
cd FigureProj.Console
dotnet run
```

## Функціональність програми

### PostgreSQL демонстрації:
1. Створення фігур та збереження в БД
2. Читання всіх фігур з БД
3. Демонстрація зв'язків (колекції, теги, метадані)
4. LINQ запити до БД
5. Пагінація
6. Оновлення даних
7. Видалення даних
8. Складні запити з навігаційними властивостями

### MongoDB демонстрації:
10. Створення документів у MongoDB
11. Читання з MongoDB
12. Запити до MongoDB (за типом, кольором, тегом)
13. Оновлення документа
14. Видалення з MongoDB
15. Порівняння SQL vs NoSQL

## ERD діаграма PostgreSQL

```
┌──────────────┐
│  Collections │
│──────────────│
│ Id (PK)      │
│ Name         │
│ Description  │
│ CreatedAt    │
└──────────────┘
       │
       │ 1:N
       ↓
┌──────────────────────┐          ┌──────────────────┐
│      Figures         │  1:1     │  FigureMetadata  │
│──────────────────────│◄─────────│──────────────────│
│ Id (PK)              │          │ Id (PK)          │
│ Name                 │          │ FigureId (FK)    │
│ Color                │          │ Author           │
│ Area                 │          │ Description      │
│ Perimeter            │          │ CreatedBy        │
│ CreatedAt            │          │ LastModified     │
│ CollectionId (FK)    │          └──────────────────┘
└──────────────────────┘
       △
       │ TPT
       ├─────────────┬─────────────┬─────────────┐
       │             │             │             │
┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐
│ Circles  │  │Rectangles│  │ Squares  │  │Triangles │
│──────────│  │──────────│  │──────────│  │──────────│
│ Id (PK)  │  │ Id (PK)  │  │ Id (PK)  │  │ Id (PK)  │
│ Radius   │  │ Height   │  │ Side     │  │ A        │
└──────────┘  │ Width    │  └──────────┘  │ B        │
              └──────────┘                 │ C        │
                                           └──────────┘

┌──────────────────────┐     ┌──────────────┐     ┌──────────┐
│     FigureTags       │ N:M │   Figures    │ N:M │   Tags   │
│──────────────────────│─────┤              │─────│──────────│
│ FigureId (PK, FK)    │     │              │     │ Id (PK)  │
│ TagId (PK, FK)       │     │              │     │ Name     │
│ AssignedAt           │     └──────────────┘     │ Color    │
└──────────────────────┘                          │CreatedAt │
                                                   └──────────┘
```

## Ключові особливості реалізації

### Fluent API конфігурація

```csharp
// Table-per-Type
modelBuilder.Entity<FigureModel>().ToTable("Figures");
modelBuilder.Entity<CircleModel>().ToTable("Circles").HasBaseType<FigureModel>();

// Зв'язок один-до-одного
modelBuilder.Entity<FigureModel>()
    .HasOne(f => f.Metadata)
    .WithOne(m => m.Figure)
    .HasForeignKey<FigureMetadataModel>(m => m.FigureId);

// Зв'язок один-до-багатьох
modelBuilder.Entity<CollectionModel>()
    .HasMany(c => c.Figures)
    .WithOne(f => f.Collection)
    .HasForeignKey(f => f.CollectionId);

// Зв'язок багато-до-багатьох
modelBuilder.Entity<FigureTagModel>()
    .HasKey(ft => new { ft.FigureId, ft.TagId });
```

### Repository Pattern

```csharp
public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task Update(T entity);
    Task Delete(T entity);
}
```

### Маппінг Domain ↔ Database

```csharp
// Domain → Database
public static FigureModel ToDbModel(this Figure domainFigure);

// Database → Domain
public static Figure ToDomainModel(this FigureModel dbModel);
```

## Тестування

```bash
cd FigureProj.Tests
dotnet test
```

## Інструменти для перегляду БД

### PostgreSQL:
- **pgAdmin** - офіційний GUI інструмент
- **DBeaver** - універсальний клієнт БД
- **psql** - командний рядок

### MongoDB:
- **MongoDB Compass** - офіційний GUI
- **mongosh** - MongoDB Shell
- **Studio 3T** - розширений GUI

## Корисні команди

### EF Core міграції:
```bash
# Створити міграцію
dotnet ef migrations add <MigrationName>

# Застосувати міграції
dotnet ef database update

# Видалити останню міграцію
dotnet ef migrations remove

# Видалити базу даних
dotnet ef database drop
```

### PostgreSQL (psql):
```bash
# Підключитися
psql -U postgres -d figuredb

# Список таблиць
\dt

# Структура таблиці
\d Figures

# Вибірка даних
SELECT * FROM "Figures" LIMIT 10;
```

### MongoDB (mongosh):
```bash
# Підключитися
mongosh

# Використати БД
use figuredb_nosql

# Список колекцій
show collections

# Вибірка документів
db.figures.find().pretty()
```

## Автор

Роман Чорнорук
Група: 404 ТН
Дата: Грудень 2025

## Додаткова інформація

Детальні інструкції зі створення звіту та скріншотів див. у файлі `LAB3_INSTRUCTIONS.md`.

