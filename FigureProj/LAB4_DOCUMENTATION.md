# Лабораторна робота №4
## Розробка вебсерверів засобами фреймворку ASP.NET

**Виконав:** Чорнорук Роман  
**Група:** 404-TN  
**Дата:** 23 грудня 2025

---

## Зміст

1. [Опис проєкту](#опис-проєкту)
2. [Архітектура](#архітектура)
3. [Моделі (DTO)](#моделі-dto)
4. [Контролери](#контролери)
5. [Dependency Injection](#dependency-injection)
6. [Тестування API](#тестування-api)
7. [Висновки](#висновки)

---

## Опис проєкту

Проєкт **FigureProj.REST** - це RESTful Web API для роботи з геометричними фігурами, створений на базі ASP.NET Core 8.0.

### Основні можливості:
- ✅ CRUD операції для фігур (Circle, Rectangle, Square, Triangle)
- ✅ CRUD операції для колекцій фігур
- ✅ Пагінація результатів
- ✅ Валідація даних
- ✅ Swagger документація
- ✅ Dependency Injection
- ✅ Робота з PostgreSQL через Entity Framework Core

---

## Архітектура

Проєкт побудований за **3-рівневою архітектурою**:

### 1. Presentation Layer (FigureProj.REST)
- **Controllers** - обробка HTTP запитів
- **Models (DTO)** - моделі для передачі даних

### 2. Business Logic Layer (FigureProj.Common)
- **Domain Models** - доменні моделі (Circle, Rectangle, Square, Triangle)
- **Services** - бізнес-логіка (ICrudServiceAsync)

### 3. Data Access Layer (FigureProj.Infrastructure)
- **Database Models** - моделі для БД
- **Repositories** - доступ до даних
- **DbContext** - контекст Entity Framework

### Переваги 3-рівневої архітектури:
- 🔹 **Розділення відповідальності** - кожен рівень має свою задачу
- 🔹 **Незалежність** - зміни в одному рівні не впливають на інші
- 🔹 **Тестованість** - легко тестувати кожен рівень окремо
- 🔹 **Масштабованість** - легко додавати нову функціональність
- 🔹 **Повторне використання** - сервіси можна використовувати в різних проєктах

---

## Моделі (DTO)

### Навіщо потрібні окремі DTO моделі?

**DTO (Data Transfer Object)** - це моделі, які використовуються для передачі даних між клієнтом і сервером.

**Різниця між DTO та сутностями БД:**

| Аспект | DTO моделі | Сутності БД |
|--------|-----------|-------------|
| Призначення | Передача даних через API | Зберігання в БД |
| Властивості | Тільки необхідні для API | Всі властивості + навігаційні |
| Валідація | Валідація вхідних даних | Обмеження БД |
| Безпека | Приховують внутрішню структуру | Містять всі дані |

### Створені DTO моделі:

#### 1. FigureDto (для читання)
```csharp
public class FigureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public double Area { get; set; }
    public double Perimeter { get; set; }
    public string Type { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### 2. CreateFigureDto (для створення)
```csharp
public class CreateFigureDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    
    [Required]
    public string Color { get; set; }
    
    [Required]
    public string Type { get; set; }
    
    // Властивості для різних типів фігур
    public double? Radius { get; set; }
    public double? Width { get; set; }
    public double? Height { get; set; }
    public double? SideA { get; set; }
    public double? SideB { get; set; }
    public double? SideC { get; set; }
}
```

#### 3. UpdateFigureDto (для оновлення)
```csharp
public class UpdateFigureDto
{
    [Required]
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }
    
    [Required]
    public string Color { get; set; }
}
```

#### 4. CollectionDto, CreateCollectionDto, UpdateCollectionDto
Аналогічні моделі для роботи з колекціями фігур.

---

## Контролери

### FiguresController

Контролер для роботи з фігурами, реалізує всі CRUD операції.

#### Основні методи:

| HTTP метод | Endpoint | Дія | HTTP код відповіді |
|------------|----------|-----|-------------------|
| GET | /api/figures | Отримати всі фігури | 200 OK |
| GET | /api/figures/{id} | Отримати фігуру за ID | 200 OK / 404 Not Found |
| GET | /api/figures/page | Пагінація | 200 OK / 400 Bad Request |
| POST | /api/figures | Створити фігуру | 201 Created / 400 Bad Request |
| PUT | /api/figures/{id} | Оновити фігуру | 200 OK / 404 Not Found |
| DELETE | /api/figures/{id} | Видалити фігуру | 204 No Content / 404 Not Found |

#### Приклад методу створення:

```csharp
[HttpPost]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
public async Task<ActionResult<FigureDto>> Create([FromBody] CreateFigureDto createDto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var figure = CreateFigureFromDto(createDto);
    await _figureService.CreateAsync(figure);
    var dto = MapToDto(figure);
    
    return CreatedAtAction(nameof(GetById), new { id = figure.Id }, dto);
}
```

### CollectionsController

Контролер для роботи з колекціями фігур.

#### Основні методи:

| HTTP метод | Endpoint | Дія | HTTP код відповіді |
|------------|----------|-----|-------------------|
| GET | /api/collections | Отримати всі колекції | 200 OK |
| GET | /api/collections/{id} | Отримати колекцію за ID | 200 OK / 404 Not Found |
| GET | /api/collections/{id}/figures | Отримати фігури в колекції | 200 OK / 404 Not Found |
| POST | /api/collections | Створити колекцію | 201 Created / 400 Bad Request |
| PUT | /api/collections/{id} | Оновити колекцію | 200 OK / 404 Not Found |
| DELETE | /api/collections/{id} | Видалити колекцію | 204 No Content / 404 Not Found |

---

## Dependency Injection

### Що таке Dependency Injection?

**Dependency Injection (DI)** - це патерн проєктування, який дозволяє передавати залежності об'єкту ззовні, замість того, щоб об'єкт створював їх сам.

### Налаштування в Program.cs:

```csharp
// Підключення до PostgreSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FigureContext>(options =>
    options.UseNpgsql(connectionString));

// Реєстрація репозиторіїв та сервісів
builder.Services.AddScoped<IRepository<FigureModel>, FigureRepository>();
builder.Services.AddScoped<ICrudServiceAsync<Figure>, DbCrudServiceAsync<Figure>>();
```

### Використання в контролері:

```csharp
public class FiguresController : ControllerBase
{
    private readonly ICrudServiceAsync<Figure> _figureService;
    private readonly ILogger<FiguresController> _logger;

    public FiguresController(
        ICrudServiceAsync<Figure> figureService,
        ILogger<FiguresController> logger)
    {
        _figureService = figureService;
        _logger = logger;
    }
}
```

### Переваги DI:
- ✅ Слабка зв'язаність (loose coupling)
- ✅ Легке тестування (можна підставити mock об'єкти)
- ✅ Гнучкість (легко замінити реалізацію)
- ✅ Керування життєвим циклом об'єктів

---

## Тестування API

### 1. Swagger UI

API автоматично генерує Swagger документацію, доступну за адресою: `http://localhost:5000`

### 2. Тестування через PowerShell

Створено скрипт `test_api.ps1` для автоматизованого тестування всіх endpoints.

### Результати тестування:

#### ✅ Тест 1: Створення кола (POST /api/figures)
```json
{
  "name": "Test Circle",
  "color": "chervonyi",
  "type": "Circle",
  "radius": 15.5
}
```
**Результат:** 201 Created  
**ID:** b82703af-056a-4107-90ac-351db4fe8d77

#### ✅ Тест 2: Створення прямокутника (POST /api/figures)
```json
{
  "name": "Test Rectangle",
  "color": "synii",
  "type": "Rectangle",
  "width": 20.0,
  "height": 10.0
}
```
**Результат:** 201 Created  
**ID:** 7c9b6708-3715-4cd3-976d-81a039ec3838

#### ✅ Тест 3: Створення квадрата (POST /api/figures)
```json
{
  "name": "Test Square",
  "color": "zelenyi",
  "type": "Square",
  "width": 12.0
}
```
**Результат:** 201 Created  
**ID:** c592ce77-328d-4ad2-9463-65a594c902a9

#### ✅ Тест 4: Створення трикутника (POST /api/figures)
```json
{
  "name": "Test Triangle",
  "color": "zhovtyi",
  "type": "Triangle",
  "sideA": 5.0,
  "sideB": 6.0,
  "sideC": 7.0
}
```
**Результат:** 201 Created  
**ID:** 98e47389-5db5-4a6b-9875-82d543e2ac8f

#### ✅ Тест 5: Отримання всіх фігур (GET /api/figures)
**Результат:** 200 OK  
**Знайдено фігур:** 11

#### ✅ Тест 6: Отримання фігури за ID (GET /api/figures/{id})
**Результат:** 200 OK
```json
{
  "id": "b82703af-056a-4107-90ac-351db4fe8d77",
  "name": "Test Circle",
  "color": "chervonyi",
  "type": "Circle",
  "area": 754.77,
  "perimeter": 97.39
}
```

#### ✅ Тест 7: Оновлення фігури (PUT /api/figures/{id})
```json
{
  "id": "7c9b6708-3715-4cd3-976d-81a039ec3838",
  "name": "Updated Rectangle",
  "color": "fioletovyi"
}
```
**Результат:** 200 OK

#### ✅ Тест 8: Пагінація (GET /api/figures/page?page=1&pageSize=2)
**Результат:** 200 OK  
**Отримано:** 2 фігури

#### ✅ Тест 9: Створення колекції (POST /api/collections)
```json
{
  "name": "Test Collection",
  "description": "Collection for API testing"
}
```
**Результат:** 201 Created  
**ID:** 2

#### ✅ Тест 10: Отримання всіх колекцій (GET /api/collections)
**Результат:** 200 OK  
**Знайдено колекцій:** 2

#### ✅ Тест 11: Оновлення колекції (PUT /api/collections/{id})
```json
{
  "id": 2,
  "name": "Updated Collection",
  "description": "Updated description"
}
```
**Результат:** 200 OK

#### ✅ Тест 12: Видалення фігури (DELETE /api/figures/{id})
**Результат:** 204 No Content

#### ✅ Тест 13: Перевірка видалення (GET /api/figures/{id})
**Результат:** 404 Not Found (очікувано)

---

## REST API та принцип однорідності інтерфейсу

### Що таке REST?

**REST (Representational State Transfer)** - це архітектурний стиль для розподілених систем, заснований на HTTP протоколі.

### 4-а умова Філдинга: Однорідність інтерфейсу (Uniform Interface)

Наш API дотримується цього принципу:

#### 1. Унікальні назви ресурсів
- `/api/figures` - колекція фігур
- `/api/figures/{id}` - конкретна фігура
- `/api/collections` - колекція колекцій
- `/api/collections/{id}` - конкретна колекція

#### 2. Правильні HTTP методи

| Метод | Призначення | Ідемпотентний? |
|-------|-------------|----------------|
| GET | Читання | Так |
| POST | Створення | Ні |
| PUT | Оновлення | Так |
| DELETE | Видалення | Так |

#### 3. Правильні HTTP коди відповідей

| Код | Значення | Використання |
|-----|----------|--------------|
| 200 OK | Успішно | GET, PUT |
| 201 Created | Створено | POST |
| 204 No Content | Без вмісту | DELETE |
| 400 Bad Request | Помилка валідації | POST, PUT |
| 404 Not Found | Не знайдено | GET, PUT, DELETE |

#### 4. Самоописовість повідомлень
- Використання JSON для передачі даних
- Content-Type: application/json
- Swagger документація

---

## Висновки

### Виконані завдання:

✅ Створено проєкт ASP.NET Core Web API  
✅ Створено папку Models з DTO моделями  
✅ Створено контролери FiguresController та CollectionsController  
✅ Реалізовано CRUD операції для двох сутностей  
✅ API відповідає принципу однорідності інтерфейсу  
✅ Використано асинхронний CRUD сервіс з 3-ї лабораторної  
✅ Налаштовано Dependency Injection  
✅ Протестовано всі endpoints  
✅ Створено документацію

### Ключові особливості ASP.NET Core Web API:

1. **Легковісність** - мінімальний overhead
2. **Продуктивність** - висока швидкість обробки запитів
3. **Кросплатформеність** - працює на Windows, Linux, macOS
4. **Вбудований DI** - Dependency Injection з коробки
5. **Swagger** - автоматична генерація документації
6. **Middleware pipeline** - гнучка обробка запитів

### Різниця між Web API та MVC/Blazor:

| Аспект | Web API | MVC | Blazor |
|--------|---------|-----|--------|
| Призначення | REST API | Web додатки | SPA додатки |
| Відповідь | JSON/XML | HTML | HTML + C# |
| Клієнт | Будь-який | Браузер | Браузер |
| Rendering | Немає | Server-side | Client/Server |

### Набуті навички:

- 📚 Розробка RESTful API
- 📚 Робота з DTO моделями
- 📚 Dependency Injection в ASP.NET Core
- 📚 Валідація даних
- 📚 Тестування API
- 📚 Документування API через Swagger

---

## Контрольні запитання

### 1. Які ключові особливості шаблону ASP.NET Core Web API та чим він відрізняється від Blazor Web App або ASP.NET MVC?

**Відповідь:**

**ASP.NET Core Web API:**
- Призначений для створення RESTful сервісів
- Повертає дані в форматі JSON/XML
- Не має UI (View)
- Використовується для створення backend API
- Може обслуговувати різні типи клієнтів (web, mobile, desktop)

**Blazor Web App:**
- Призначений для створення інтерактивних web додатків
- Використовує C# замість JavaScript
- Має компоненти UI
- Може працювати на клієнті (WebAssembly) або сервері (Server-side)

**ASP.NET MVC:**
- Призначений для створення традиційних web додатків
- Використовує патерн Model-View-Controller
- Генерує HTML на сервері
- Має View для відображення даних

### 2. Поясніть, як реалізується трирівнева архітектура у вашому проєкті FigureProj.REST та які переваги вона має.

**Відповідь:**

**Рівні архітектури:**

1. **Presentation Layer (FigureProj.REST)**
   - Controllers - обробка HTTP запитів
   - DTO Models - моделі для API

2. **Business Logic Layer (FigureProj.Common)**
   - Domain Models - бізнес-об'єкти
   - Services - бізнес-логіка

3. **Data Access Layer (FigureProj.Infrastructure)**
   - Database Models - моделі БД
   - Repositories - доступ до даних
   - DbContext - контекст EF Core

**Переваги:**
- Розділення відповідальності
- Незалежність рівнів
- Легке тестування
- Масштабованість
- Повторне використання коду

### 3. Навіщо створювати окремі моделі у папці Models та в чому різниця між цими моделями й сутностями, що зберігаються в базі даних?

**Відповідь:**

**DTO моделі (Models):**
- Використовуються для передачі даних через API
- Містять тільки необхідні властивості
- Мають валідацію для вхідних даних
- Приховують внутрішню структуру БД
- Забезпечують безпеку (не передають зайві дані)

**Сутності БД (Database Models):**
- Використовуються для зберігання в БД
- Містять всі властивості + навігаційні
- Мають обмеження БД
- Відображають структуру таблиць
- Можуть містити службові поля (CreatedAt, UpdatedAt)

**Приклад:**
- CreateFigureDto не має ID (він генерується при створенні)
- UpdateFigureDto не має властивостей геометрії (їх не можна змінити)
- FigureDto не має навігаційних властивостей (Collection, Metadata)

### 4. Що таке REST і як у вашому API забезпечується дотримання принципу однорідності інтерфейсу (uniform interface)?

**Відповідь:**

**REST (Representational State Transfer)** - це архітектурний стиль для розподілених систем.

**Принцип однорідності інтерфейсу забезпечується через:**

1. **Унікальні URI для ресурсів:**
   - `/api/figures` - колекція
   - `/api/figures/{id}` - конкретний ресурс

2. **Правильні HTTP методи:**
   - GET - читання
   - POST - створення
   - PUT - оновлення
   - DELETE - видалення

3. **Правильні HTTP коди:**
   - 200 OK - успішно
   - 201 Created - створено
   - 204 No Content - видалено
   - 400 Bad Request - помилка валідації
   - 404 Not Found - не знайдено

4. **Самоописовість:**
   - JSON формат
   - Content-Type заголовки
   - Swagger документація

### 5. Які HTTP-методи використовуються для CRUD-операцій? Наведіть приклади відповідності методів діям у контролерах.

**Відповідь:**

| CRUD операція | HTTP метод | Endpoint | Метод контролера |
|---------------|------------|----------|------------------|
| **Create** | POST | /api/figures | Create() |
| **Read** | GET | /api/figures | GetAll() |
| **Read** | GET | /api/figures/{id} | GetById(id) |
| **Update** | PUT | /api/figures/{id} | Update(id, dto) |
| **Delete** | DELETE | /api/figures/{id} | Delete(id) |

**Приклади:**

```csharp
// CREATE
[HttpPost]
public async Task<ActionResult<FigureDto>> Create([FromBody] CreateFigureDto dto)

// READ
[HttpGet]
public async Task<ActionResult<IEnumerable<FigureDto>>> GetAll()

[HttpGet("{id}")]
public async Task<ActionResult<FigureDto>> GetById(Guid id)

// UPDATE
[HttpPut("{id}")]
public async Task<ActionResult<FigureDto>> Update(Guid id, [FromBody] UpdateFigureDto dto)

// DELETE
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(Guid id)
```

---

**Дата виконання:** 23 грудня 2025  
**Виконав:** Чорнорук Роман, група 404-TN

