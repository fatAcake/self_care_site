# Docker для Self Care API

## 🐳 Локальная разработка с Docker

### Быстрый старт:

1. **Запустите PostgreSQL и API:**
   ```bash
   cd Self_care
   docker-compose up -d
   ```

2. **Выполните миграции:**
   ```bash
   docker-compose exec api dotnet ef database update
   ```

3. **API будет доступен на:** `http://localhost:8080`

4. **Остановка:**
   ```bash
   docker-compose down
   ```

---

## 📦 Сборка Docker образа вручную

### Сборка:
```bash
cd Self_care
docker build -t self-care-api .
```

### Запуск:
```bash
docker run -p 8080:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__DefaultConnection="Host=your-host;Database=self_careDB;Username=user;Password=pass;SSL Mode=Require" \
  -e Jwt__Key="your-secret-key-minimum-32-characters" \
  -e Jwt__Issuer=Self_care \
  -e Jwt__Audience=SelfCare.Front \
  -e FRONTEND_URL="https://your-frontend-url.com" \
  self-care-api
```

---

## 🚀 Деплой на Render

Render автоматически обнаружит Dockerfile и соберет образ.

**Настройки в Render:**
- Environment: **Docker**
- Root Directory: `Self_care`
- Dockerfile Path: `Dockerfile` (или оставьте пустым)

---

## 🔧 Переменные окружения

Все переменные окружения должны быть установлены в настройках Render:

- `ASPNETCORE_ENVIRONMENT=Production`
- `ConnectionStrings__DefaultConnection=...`
- `Jwt__Key=...`
- `Jwt__Issuer=Self_care`
- `Jwt__Audience=SelfCare.Front`
- `FRONTEND_URL=...`

---

## 📝 Структура Dockerfile

1. **Build stage**: Собирает приложение
2. **Publish stage**: Публикует приложение
3. **Final stage**: Создает минимальный runtime образ

Используется многоступенчатая сборка для уменьшения размера финального образа.

---

## 🐛 Отладка

### Просмотр логов:
```bash
docker-compose logs -f api
```

### Вход в контейнер:
```bash
docker-compose exec api sh
```

### Пересборка после изменений:
```bash
docker-compose up -d --build
```

