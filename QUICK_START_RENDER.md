# 🚀 Быстрый старт на Render

## 📝 Что нужно сделать:

### 1. **Создайте PostgreSQL базу данных**
- Зайдите на render.com → New + → PostgreSQL
- Name: `self-care-db`
- Скопируйте **Internal Database URL**

### 2. **Создайте Backend API**
- New + → Web Service
- Environment: **Dotnet**
- Build Command: `dotnet restore && dotnet publish -c Release -o ./publish`
- Start Command: `cd publish && dotnet Self_care.dll`

**Environment Variables:**
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<ваш Internal Database URL>
Jwt__Key=<сгенерируйте случайную строку минимум 32 символа>
Jwt__Issuer=Self_care
Jwt__Audience=SelfCare.Front
FRONTEND_URL=https://your-frontend-name.onrender.com
```

### 3. **Создайте Frontend**
- New + → Static Site
- Root Directory: `self_care_front`
- Build Command: `npm install && npm run build`
- Publish Directory: `dist`

**Environment Variables:**
```
VITE_API_BASE_URL=https://your-api-name.onrender.com/api
```

### 4. **Обновите CORS**
После создания фронтенда вернитесь в Backend и обновите:
```
FRONTEND_URL=https://your-actual-frontend-url.onrender.com
```

### 5. **Выполните миграции**
В Shell вашего Backend API:
```bash
cd /opt/render/project/src
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

## 📄 Файлы для справки:
- `.env.example` - примеры переменных окружения
- `RENDER_DEPLOY.md` - подробная инструкция
- `render.yaml` - конфигурация для автоматического деплоя (опционально)

---

## ⚠️ Важно:
1. **Не коммитьте** `.env` файлы в Git
2. Используйте **Internal Database URL** для подключения
3. Генерируйте **уникальный JWT ключ** (можно использовать: `openssl rand -base64 32`)
4. После первого деплоя обновите `FRONTEND_URL` в Backend

---

## 🔗 Полезные ссылки:
- [Render Docs](https://render.com/docs)
- [.NET на Render](https://render.com/docs/dotnet)
- [Static Sites на Render](https://render.com/docs/static-sites)

