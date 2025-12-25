# 📋 Руководство по миграциям Entity Framework

## 🚀 Быстрый старт

### Предварительные требования:
1. **Установлен .NET 9.0 SDK**
2. **PostgreSQL запущен локально** (или через Docker)
3. **База данных создана** (или будет создана автоматически)

---

## 📝 Создание первой миграции

### 1. Убедитесь, что PostgreSQL запущен

**Если используете Docker:**
```bash
docker-compose up -d db
```

**Или запустите PostgreSQL локально** и убедитесь, что он доступен на `localhost:5432`

### 2. Проверьте connection string

В файле `appsettings.json` должна быть строка подключения:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=self_careDB;Username=postgres;Password=1234"
  }
}
```

### 3. Создайте миграцию

Откройте терминал в папке `Self_care` и выполните:

```bash
dotnet ef migrations add InitialCreate
```

Это создаст папку `Migrations` с файлами миграции.

---

## 🔄 Применение миграций к базе данных

### Применить все миграции:
```bash
dotnet ef database update
```

### Применить конкретную миграцию:
```bash
dotnet ef database update MigrationName
```

### Откатить последнюю миграцию:
```bash
dotnet ef database update PreviousMigrationName
```

---

## 📦 Создание новой миграции после изменений моделей

Если вы изменили модели (добавили/удалили поля, таблицы и т.д.):

1. **Создайте новую миграцию:**
   ```bash
   dotnet ef migrations add AddNewFieldToUser
   ```
   (замените `AddNewFieldToUser` на описательное имя)

2. **Примените миграцию:**
   ```bash
   dotnet ef database update
   ```

---

## 🗑️ Удаление миграции

### Удалить последнюю миграцию (если она еще не применена):
```bash
dotnet ef migrations remove
```

⚠️ **Внимание**: Это удалит только файлы миграции. Если миграция уже применена к БД, сначала откатите её.

---

## 🔍 Полезные команды

### Просмотр списка миграций:
```bash
dotnet ef migrations list
```

### Просмотр SQL, который будет выполнен:
```bash
dotnet ef migrations script
```

### Создать SQL скрипт для всех миграций:
```bash
dotnet ef migrations script -o migrations.sql
```

### Создать SQL скрипт от одной миграции до другой:
```bash
dotnet ef migrations script FromMigration ToMigration -o update.sql
```

---

## 🐳 Миграции с Docker

### Если используете docker-compose:

1. **Запустите контейнеры:**
   ```bash
   docker-compose up -d
   ```

2. **Выполните миграции внутри контейнера:**
   ```bash
   docker-compose exec api dotnet ef database update
   ```

   Или если контейнер еще не запущен:
   ```bash
   docker-compose run --rm api dotnet ef database update
   ```

---

## ⚙️ Настройка для разных окружений

### Development (локально):
Используется `appsettings.json` или `appsettings.Development.json`

### Production:
Используйте переменные окружения:
```bash
$env:ConnectionStrings__DefaultConnection="Host=localhost;Database=self_careDB;Username=postgres;Password=1234"
dotnet ef database update
```

Или в PowerShell:
```powershell
$env:ConnectionStrings__DefaultConnection="Host=localhost;Database=self_careDB;Username=postgres;Password=1234"
dotnet ef database update
```

---

## 🐛 Решение проблем

### Ошибка: "No DbContext was found"
Убедитесь, что вы находитесь в папке `Self_care` и что `Self_care.csproj` существует.

### Ошибка: "Unable to connect to database"
1. Проверьте, что PostgreSQL запущен
2. Проверьте connection string в `appsettings.json`
3. Убедитесь, что база данных существует (или создайте её вручную)

### Ошибка: "Package 'Microsoft.EntityFrameworkCore.Design' not found"
Установите пакет:
```bash
dotnet add package Microsoft.EntityFrameworkCore.Design
```

### Ошибка: "dotnet ef command not found"
Установите EF Core tools глобально:
```bash
dotnet tool install --global dotnet-ef
```

Или используйте через `dotnet`:
```bash
dotnet tool run dotnet-ef migrations add InitialCreate
```

---

## 📁 Структура после создания миграций

После выполнения `dotnet ef migrations add InitialCreate` будет создана структура:

```
Self_care/
├── Migrations/
│   ├── 20240101120000_InitialCreate.cs
│   ├── 20240101120000_InitialCreate.Designer.cs
│   └── SelfCareDBModelSnapshot.cs
├── Self_care.csproj
└── ...
```

---

## ✅ Чек-лист перед применением миграций

- [ ] PostgreSQL запущен и доступен
- [ ] Connection string правильный в `appsettings.json`
- [ ] База данных существует (или будет создана автоматически)
- [ ] Все изменения в моделях сохранены
- [ ] Проект компилируется без ошибок

---

## 🔐 Безопасность

⚠️ **Важно**: 
- Не коммитьте connection strings с паролями в Git
- Используйте переменные окружения для production
- Делайте резервные копии базы данных перед применением миграций в production

---

## 📚 Дополнительные ресурсы

- [Entity Framework Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
- [PostgreSQL с EF Core](https://www.npgsql.org/efcore/)

