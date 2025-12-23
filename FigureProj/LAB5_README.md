# Лабораторна робота №5 - ASP.NET Core Identity & JWT Authentication

## Огляд реалізації

Проєкт FigureProj було розширено можливостями реєстрації, аутентифікації та авторизації з використанням ASP.NET Core Identity та JWT токенів.

## Що було реалізовано

### 1. Інтеграція ASP.NET Core Identity

- **Пакети:**
  - `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (Infrastructure)
  - `Microsoft.AspNetCore.Authentication.JwtBearer` (REST API)
  
- **Сутність користувача:** `ApplicationUser` (наслідує `IdentityUser`)
  - Додаткові поля: `FullName`, `CreatedAt`, `LastLogin`
  
- **DbContext:** Модифікований `FigureContext` наслідує `IdentityDbContext<ApplicationUser>`
  - Автоматично додані таблиці: AspNetUsers, AspNetRoles, AspNetUserRoles, etc.

### 2. JWT Authentication

**Налаштування (appsettings.json):**
```json
{
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyForJwtTokenGeneration2024!MustBeLongEnough",
    "Issuer": "FigureProj.REST",
    "Audience": "FigureProj.Client",
    "ExpirationDays": 7
  }
}
```

### 3. Система ролей

Реалізовано 3 ролі з різними рівнями доступу:

| Роль | Опис | Повноваження |
|------|------|--------------|
| **Administrator** | Адміністратор системи | Повний доступ до всіх операцій |
| **Editor** | Редактор контенту | Може створювати та редагувати |
| **Viewer** | Переглядач | Може тільки переглядати та створювати |

### 4. REST API Endpoints з авторизацією

#### Фігури (`/api/figures`)
- `GET /api/figures` - Публічний доступ
- `GET /api/figures/{id}` - Публічний доступ
- `POST /api/figures` - Авторизовані користувачі
- `PUT /api/figures/{id}` - Editor, Administrator
- `DELETE /api/figures/{id}` - Тільки Administrator

#### Колекції (`/api/collections`)
- `GET /api/collections` - Публічний доступ
- `GET /api/collections/{id}` - Публічний доступ
- `POST /api/collections` - Авторизовані користувачі
- `PUT /api/collections/{id}` - Editor, Administrator
- `DELETE /api/collections/{id}` - Тільки Administrator

#### Аутентифікація (`/api/auth`)
- `POST /api/auth/register` - Реєстрація нового користувача
- `POST /api/auth/login` - Вхід та отримання JWT токену
- `GET /api/auth/me` - Інформація про поточного користувача

### 5. Swagger з підтримкою JWT

Swagger UI налаштовано для роботи з Bearer токенами через кнопку **Authorize**.

## Швидкий старт

### 1. Налаштування бази даних

Оновіть `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=figuredb;Username=postgres;Password=ВАШ_ПАРОЛЬ"
  }
}
```

### 2. Запуск

```bash
cd FigureProj
dotnet run --project FigureProj.REST
```

Відкрийте: `http://localhost:5000`

### 3. Тестування

**Користувач за замовчуванням:**
- Email: `admin@figureproj.com`
- Пароль: `Admin123!`
- Роль: Administrator

## Приклади запитів

### Реєстрація
```bash
POST /api/auth/register
{
  "userName": "editor",
  "email": "editor@figureproj.com",
  "password": "Editor123!",
  "fullName": "Іван Редактор",
  "role": "Editor"
}
```

### Вхід
```bash
POST /api/auth/login
{
  "email": "editor@figureproj.com",
  "password": "Editor123!"
}
```

### Використання токена
```bash
Authorization: Bearer ВАШ_JWT_ТОКЕН
```

---

**Автор:** Роман Чорнорук  
**Група:** 404-ТН  
**Дата:** Грудень 2024
