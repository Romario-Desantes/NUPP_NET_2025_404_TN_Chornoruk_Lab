# Лабораторна робота №3 - Інструкції

## Підготовка до запуску

### 1. Встановлення PostgreSQL

#### Windows:
1. Завантажте PostgreSQL з https://www.postgresql.org/download/windows/
2. Запустіть інсталятор та встановіть PostgreSQL
3. Під час встановлення запам'ятайте пароль для користувача `postgres`
4. За замовчуванням PostgreSQL буде доступний на порту 5432

#### Linux/Mac:
```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install postgresql postgresql-contrib

# MacOS (Homebrew)
brew install postgresql
brew services start postgresql
```

### 2. Налаштування PostgreSQL

1. Створіть базу даних (опціонально, додаток створить автоматично):
```sql
CREATE DATABASE figuredb;
```

2. Переконайтеся, що PostgreSQL запущено:
```bash
# Windows
pg_isready

# Linux/Mac
sudo systemctl status postgresql
```

### 3. Встановлення MongoDB (для додаткового завдання)

#### Windows:
1. Завантажте MongoDB Community Server з https://www.mongodb.com/try/download/community
2. Запустіть інсталятор
3. MongoDB буде доступний на порту 27017

#### Linux/Mac:
```bash
# Ubuntu/Debian
sudo apt-get install mongodb-org

# MacOS (Homebrew)
brew tap mongodb/brew
brew install mongodb-community
brew services start mongodb-community
```

### 4. Налаштування Connection String

Відкрийте файл `FigureProj.Console/Program.cs` та змініть рядок підключення, якщо потрібно:

```csharp
var connectionString = "Host=localhost;Database=figuredb;Username=postgres;Password=ВАШ_ПАРОЛЬ";
```

Також для MongoDB:
```csharp
var mongoConnectionString = "mongodb://localhost:27017";
```

## Запуск додатку

### Застосування міграцій

Міграції застосовуються автоматично при запуску програми. Або можна застосувати вручну:

```bash
cd FigureProj/FigureProj.Infrastructure
dotnet ef database update
```

### Запуск консольного додатку

```bash
cd FigureProj/FigureProj.Console
dotnet run
```

## Створення звіту (PDF)

Для створення PDF звіту необхідно:

### 1. Скріншоти виконання програми

Запустіть програму та зробіть скріншоти:
- Вивід консолі з результатами роботи
- Всі секції виводу (створення, читання, оновлення, видалення)
- Демонстрація зв'язків
- LINQ запити
- Роботу з MongoDB

### 2. Скріншоти бази даних PostgreSQL

#### Використання pgAdmin:
1. Запустіть pgAdmin
2. Підключіться до сервера PostgreSQL
3. Розгорніть сервер → Databases → figuredb → Schemas → public → Tables
4. Зробіть скріншоти:
   - Список всіх таблиць
   - Структура таблиць (колонки, типи даних, обмеження)
   - Дані в таблицях (кілька записів)
   - Зв'язки між таблицями (Foreign Keys)

#### Використання DBeaver:
1. Запустіть DBeaver
2. Створіть нове підключення до PostgreSQL
3. Підключіться до бази figuredb
4. Зробіть скріншоти аналогічно pgAdmin

#### Командний рядок (psql):
```bash
psql -U postgres -d figuredb

# Список таблиць
\dt

# Структура таблиці
\d Figures
\d Circles
\d Collections

# Дані з таблиць
SELECT * FROM Figures LIMIT 5;
SELECT * FROM Collections;
SELECT * FROM Tags;
SELECT * FROM FigureTags;
```

### 3. Створення ERD-діаграми

#### Метод 1: pgAdmin
1. В pgAdmin: Tools → ERD For Database
2. Виберіть таблиці для відображення
3. Експортуйте діаграму як зображення

#### Метод 2: DBeaver
1. Виберіть таблиці в навігаторі БД
2. ПКМ → View Diagram
3. Експортуйте діаграму

#### Метод 3: dbdiagram.io
1. Відкрийте https://dbdiagram.io/
2. Опишіть схему у форматі DBML:

```dbml
Table Figures {
  Id int [pk, increment]
  Name varchar(100)
  Color varchar(50)
  Area double
  Perimeter double
  CreatedAt timestamp
  CollectionId int [ref: > Collections.Id]
}

Table Circles {
  Id int [pk, ref: - Figures.Id]
  Radius double
}

Table Rectangles {
  Id int [pk, ref: - Figures.Id]
  Height double
  Width double
}

Table Squares {
  Id int [pk, ref: - Figures.Id]
  Side double
}

Table Triangles {
  Id int [pk, ref: - Figures.Id]
  A double
  B double
  C double
}

Table Collections {
  Id int [pk, increment]
  Name varchar(100)
  Description varchar(500)
  CreatedAt timestamp
}

Table FigureMetadata {
  Id int [pk, increment]
  FigureId int [ref: - Figures.Id]
  Author varchar(100)
  Description varchar(500)
  CreatedBy varchar(100)
  LastModified timestamp
}

Table Tags {
  Id int [pk, increment]
  Name varchar(50) [unique]
  Color varchar(30)
  CreatedAt timestamp
}

Table FigureTags {
  FigureId int [pk, ref: > Figures.Id]
  TagId int [pk, ref: > Tags.Id]
  AssignedAt timestamp
}
```

3. Експортуйте діаграму як PNG/PDF

### 4. Скріншоти MongoDB

#### MongoDB Compass:
1. Запустіть MongoDB Compass
2. Підключіться до mongodb://localhost:27017
3. Виберіть базу даних figuredb_nosql
4. Зробіть скріншоти:
   - Список колекцій
   - Документи в колекції figures
   - Структура документів

#### mongosh (командний рядок):
```bash
mongosh

use figuredb_nosql

# Список колекцій
show collections

# Документи
db.figures.find().pretty()

# Кількість документів
db.figures.countDocuments()

# Запити
db.figures.find({Type: "Circle"})
db.figures.find({Color: "червоний"})
```

### 5. Збірка PDF звіту

Використайте Microsoft Word, Google Docs або LaTeX для створення PDF:

**Структура звіту:**

1. **Титульна сторінка**
   - Назва: Лабораторна робота №3
   - ПІБ студента
   - Група
   - Дата

2. **Мета роботи**

3. **Скріншоти виконання консольного додатку**
   - Всі секції виводу програми

4. **PostgreSQL (Реляційна БД)**
   - Скріншот списку таблиць
   - Скріншоти структури основних таблиць
   - Скріншоти даних з таблиць
   - ERD-діаграма бази даних

5. **MongoDB (NoSQL)**
   - Скріншот колекцій
   - Скріншоти документів
   - Приклади запитів

6. **Висновки**
   - Порівняння SQL vs NoSQL
   - Переваги та недоліки кожного підходу

Збережіть як `lab3_Roman_Chornoruk.pdf` (замініть ім'я на своє)

## Корисні команди

### PostgreSQL
```bash
# Підключення
psql -U postgres -d figuredb

# Резервна копія
pg_dump -U postgres figuredb > backup.sql

# Відновлення
psql -U postgres figuredb < backup.sql
```

### MongoDB
```bash
# Підключення
mongosh

# Експорт колекції
mongoexport --db=figuredb_nosql --collection=figures --out=figures.json

# Імпорт колекції
mongoimport --db=figuredb_nosql --collection=figures --file=figures.json
```

## Troubleshooting

### PostgreSQL не підключається
- Перевірте, що сервіс запущено
- Перевірте пароль
- Перевірте порт (за замовчуванням 5432)
- Перевірте файл `pg_hba.conf` для налаштування автентифікації

### MongoDB не підключається
- Перевірте, що mongod запущено: `mongod --dbpath /path/to/data`
- Перевірте порт (за замовчуванням 27017)

### Помилки міграції
- Видаліть базу даних та застосуйте міграції знову:
```bash
dotnet ef database drop
dotnet ef database update
```

## Контрольні запитання

1. Що таке реляційні бази даних? Що таке СУБД? Які СУБД ви знаєте?
2. Що позначає термін таблиця у реляційній БД? Які існують зв'язки у реляційній БД?
3. Що таке ERD-діаграма? Як її створити та для чого вона потрібна?
4. Що таке DbContext і яку роль він відіграє в роботі з базою даних?
5. Що таке зв'язки один-до-одного, один-до-багатьох і багато-до-багатьох у контексті EF Core?
6. Як реалізувати зв'язок один-до-одного за допомогою Fluent API?
7. Наведіть приклад реалізації зв'язку багато-до-багатьох у EF Core.
8. У чому різниця між анотаціями (Data Annotations) та Fluent API?
9. Як створюється та застосовується міграція в EF Core?
10. Яка мета проєкту Infrastructure?
11. У чому різниця між доменною моделлю та моделлю БД?
12. Яку проблему вирішує шаблон Репозиторій?
13. У чому принципова різниця між реляційними та нереляційними БД?

