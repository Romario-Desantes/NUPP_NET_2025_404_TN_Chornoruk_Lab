# FigureProj.REST - ASP.NET Core Web API

REST API для роботи з геометричними фігурами (Лабораторна робота №5 - Identity & JWT Authentication).

## Запуск проєкту

### Вимоги
- .NET 8.0 SDK
- PostgreSQL (запущений на localhost:5432)
- База даних: figuredb

### Налаштування змінних оточення

Проєкт використовує змінні оточення для конфігурації. Скопіюйте `ENV_EXAMPLE` у `.env` або встановіть змінні оточення вручну.

**Обов'язкові змінні:**
- `CONNECTIONSTRINGS__DEFAULTCONNECTION` - Connection string для PostgreSQL
- `JWTSETTINGS__SECRETKEY` - Секретний ключ для JWT токенів

**Опціональні змінні:**
- `JWTSETTINGS__ISSUER` - JWT Issuer (за замовчуванням: "FigureProj.REST")
- `JWTSETTINGS__AUDIENCE` - JWT Audience (за замовчуванням: "FigureProj.Client")
- `JWTSETTINGS__EXPIRATIONDAYS` - Термін дії JWT токена в днях (за замовчуванням: 7)
- `ADMIN__EMAIL` - Email адміністратора для автоматичного створення
- `ADMIN__PASSWORD` - Пароль адміністратора для автоматичного створення

**Для локальної розробки** можна використовувати `appsettings.Development.json`.

### Запуск локально

```bash
cd FigureProj.REST
dotnet run --urls "http://localhost:5000"
```

Swagger UI буде доступний за адресою: http://localhost:5000

### Запуск з Docker

```bash
# З кореня проєкту (FigureProj)
docker build -t figureproj-api .
docker run -p 5000:5000 \
  -e CONNECTIONSTRINGS__DEFAULTCONNECTION="Host=host.docker.internal;Database=figuredb;Username=postgres;Password=your_password" \
  -e JWTSETTINGS__SECRETKEY="YourSecretKeyHere" \
  figureproj-api
```

### Розгортання на Render

1. **Підготовка:**
   - Завантажте проєкт на GitHub/GitLab/Bitbucket
   - Переконайтеся, що всі файли (включаючи `Dockerfile` та `render.yaml`) знаходяться в репозиторії

2. **Створення сервісів на Render:**
   - Використайте файл `render.yaml` для автоматичного створення сервісів
   - Або створіть сервіси вручну через веб-інтерфейс Render

3. **Налаштування Environment Variables на Render:**
   - `DATABASE_URL` - Автоматично встановлюється при підключенні PostgreSQL
   - `JWTSETTINGS__SECRETKEY` - Генеруйте надійний секретний ключ
   - `JWTSETTINGS__ISSUER` - "FigureProj.REST"
   - `JWTSETTINGS__AUDIENCE` - "FigureProj.Client"
   - `JWTSETTINGS__EXPIRATIONDAYS` - "7"
   - `ADMIN__EMAIL` - Email для адміністратора
   - `ADMIN__PASSWORD` - Пароль для адміністратора
   - `ASPNETCORE_ENVIRONMENT` - "Production"
   - `PORT` - Автоматично встановлюється Render

4. **Налаштування Build:**
   - **Dockerfile Path:** `./Dockerfile`
   - **Docker Context:** `.` (корінь репозиторію)

5. **Після розгортання:**
   - Міграції БД виконуються автоматично при старті
   - Адміністратор створюється автоматично, якщо вказані `ADMIN__EMAIL` та `ADMIN__PASSWORD`

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
- ASP.NET Core Identity
- JWT Authentication
- Swagger/OpenAPI
- Dependency Injection
- Docker

## Безпека

- Всі секрети (connection strings, JWT keys) мають бути в змінних оточення
- Ніколи не комітьте `.env` файли з реальними секретами
- Використовуйте сильні секретні ключі для JWT
- Змініть пароль адміністратора після першого входу

## Автентифікація

API використовує JWT токени для автентифікації. Для отримання токена:

1. Зареєструйтеся через `POST /api/auth/register`
2. Увійдіть через `POST /api/auth/login` (отримаєте JWT токен)
3. Використовуйте токен в заголовку: `Authorization: Bearer {token}`

### Ролі:
- **Administrator** - повний доступ до всіх операцій
- **Editor** - може створювати та редагувати фігури
- **Viewer** - тільки читання


