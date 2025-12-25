# Инструкция по деплою проекта Self Care

## 📋 Что нужно для деплоя

### 1. **Backend (ASP.NET Core 9.0)**

#### Требования:
- ✅ Сервер с поддержкой .NET 9.0 Runtime
- ✅ PostgreSQL база данных
- ✅ Переменные окружения для конфигурации

#### Варианты хостинга:
- **Azure App Service** (рекомендуется для .NET)
- **AWS Elastic Beanstalk**
- **DigitalOcean App Platform**
- **VPS сервер** (Ubuntu/Windows Server)
- **Heroku** (с поддержкой .NET)

#### Настройки для Production:

**1. Создайте `appsettings.Production.json`:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=ВАШ_ХОСТ;Database=self_careDB;Username=ВАШ_ПОЛЬЗОВАТЕЛЬ;Password=ВАШ_ПАРОЛЬ;SSL Mode=Require"
  },
  "Jwt": {
    "Key": "ВАШ_СЕКРЕТНЫЙ_КЛЮЧ_МИНИМУМ_32_СИМВОЛА_ДЛЯ_БЕЗОПАСНОСТИ",
    "Issuer": "Self_care",
    "Audience": "SelfCare.Front"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**2. Обновите `Program.cs` для Production:**
- Настройте CORS для вашего фронтенд домена
- Включите HTTPS редирект
- Настройте правильные порты

**3. Миграции базы данных:**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

---

### 2. **Frontend (React + Vite)**

#### Требования:
- ✅ Node.js 18+ для сборки
- ✅ Хостинг для статических файлов

#### Варианты хостинга:
- **Vercel** (рекомендуется для React)
- **Netlify**
- **GitHub Pages**
- **Azure Static Web Apps**
- **AWS S3 + CloudFront**
- **Firebase Hosting**

#### Настройки для Production:

**1. Создайте `.env.production`:**
```env
VITE_API_BASE_URL=https://ваш-api-домен.com/api
```

**2. Обновите `vite.config.js` для production:**
```javascript
export default defineConfig({
  plugins: [react()],
  base: '/',
  build: {
    outDir: 'dist',
    assetsDir: 'assets',
  },
  // Уберите proxy для production
})
```

**3. Обновите API URL в коде:**
Создайте файл `self_care_front/src/config.js`:
```javascript
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || '/api';
```

Затем обновите все файлы в `src/api/`:
```javascript
// Вместо: const API_BASE_URL = '/api';
// Используйте: import { API_BASE_URL } from '../config';
```

**4. Сборка проекта:**
```bash
cd self_care_front
npm install
npm run build
```
Результат будет в папке `dist/`

---

## 🚀 Пошаговая инструкция деплоя

### Вариант 1: Azure (Backend) + Vercel (Frontend)

#### Backend на Azure:

1. **Создайте Azure App Service:**
   - Зайдите на portal.azure.com
   - Создайте новый App Service
   - Выберите .NET 9.0 runtime stack

2. **Настройте PostgreSQL:**
   - Создайте Azure Database for PostgreSQL
   - Скопируйте connection string

3. **Настройте переменные окружения в Azure:**
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `ConnectionStrings__DefaultConnection=ваша_строка_подключения`
   - `Jwt__Key=ваш_секретный_ключ`
   - `Jwt__Issuer=Self_care`
   - `Jwt__Audience=SelfCare.Front`

4. **Деплой:**
   ```bash
   cd Self_care
   dotnet publish -c Release
   # Загрузите файлы из bin/Release/net9.0/publish/ в Azure
   ```

5. **Обновите CORS в Program.cs:**
   ```csharp
   app.UseCors(policy => policy
       .WithOrigins("https://ваш-фронтенд-домен.vercel.app")
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowCredentials());
   ```

#### Frontend на Vercel:

1. **Подключите GitHub репозиторий к Vercel**

2. **Настройте переменные окружения:**
   - `VITE_API_BASE_URL=https://ваш-api.azurewebsites.net/api`

3. **Настройки сборки:**
   - Build Command: `npm run build`
   - Output Directory: `dist`
   - Install Command: `npm install`

4. **Деплой автоматический при push в GitHub**

---

### Вариант 2: VPS сервер (Ubuntu)

#### Backend:

1. **Установите .NET 9.0:**
   ```bash
   wget https://dot.net/v1/dotnet-install.sh
   chmod +x dotnet-install.sh
   ./dotnet-install.sh --version 9.0.0
   ```

2. **Установите PostgreSQL:**
   ```bash
   sudo apt update
   sudo apt install postgresql postgresql-contrib
   ```

3. **Создайте базу данных:**
   ```bash
   sudo -u postgres psql
   CREATE DATABASE self_careDB;
   CREATE USER selfcare_user WITH PASSWORD 'ваш_пароль';
   GRANT ALL PRIVILEGES ON DATABASE self_careDB TO selfcare_user;
   ```

4. **Настройте Nginx как reverse proxy:**
   ```nginx
   server {
       listen 80;
       server_name ваш-домен.com;
       
       location / {
           proxy_pass http://localhost:5000;
           proxy_http_version 1.1;
           proxy_set_header Upgrade $http_upgrade;
           proxy_set_header Connection keep-alive;
           proxy_set_header Host $host;
       }
   }
   ```

5. **Создайте systemd service:**
   ```ini
   [Unit]
   Description=Self Care API
   
   [Service]
   WorkingDirectory=/var/www/selfcare-api
   ExecStart=/usr/bin/dotnet /var/www/selfcare-api/Self_care.dll
   Restart=always
   RestartSec=10
   
   [Install]
   WantedBy=multi-user.target
   ```

#### Frontend:

1. **Соберите проект:**
   ```bash
   cd self_care_front
   npm install
   npm run build
   ```

2. **Настройте Nginx для статических файлов:**
   ```nginx
   server {
       listen 80;
       server_name ваш-фронтенд-домен.com;
       root /var/www/selfcare-frontend/dist;
       index index.html;
       
       location / {
           try_files $uri $uri/ /index.html;
       }
   }
   ```

---

## 🔐 Безопасность для Production

### Backend:

1. **Используйте сильный JWT ключ** (минимум 32 символа)
2. **Включите HTTPS** (SSL сертификат)
3. **Настройте CORS** только для вашего фронтенд домена
4. **Используйте переменные окружения** для секретов
5. **Отключите Swagger** в production:
   ```csharp
   if (app.Environment.IsDevelopment())
   {
       app.UseSwagger();
       app.UseSwaggerUI();
   }
   ```

### Frontend:

1. **Используйте HTTPS** для всех запросов
2. **Настройте правильный API URL** через переменные окружения
3. **Не храните секреты** в коде

---

## 📝 Чек-лист перед деплоем

### Backend:
- [ ] Создана production база данных PostgreSQL
- [ ] Настроен `appsettings.Production.json`
- [ ] Обновлен CORS для фронтенд домена
- [ ] Настроен сильный JWT ключ
- [ ] Выполнены миграции базы данных
- [ ] Настроен HTTPS
- [ ] Swagger отключен в production

### Frontend:
- [ ] Создан `.env.production` с правильным API URL
- [ ] Обновлен `vite.config.js` для production
- [ ] Проект успешно собирается (`npm run build`)
- [ ] Все API запросы используют правильный URL
- [ ] Проверена работа на локальной сборке (`npm run preview`)

---

## 🛠️ Полезные команды

### Backend:
```bash
# Сборка для production
dotnet publish -c Release -o ./publish

# Миграции
dotnet ef migrations add MigrationName
dotnet ef database update

# Запуск
dotnet run --environment Production
```

### Frontend:
```bash
# Установка зависимостей
npm install

# Сборка для production
npm run build

# Предпросмотр production сборки
npm run preview

# Проверка линтера
npm run lint
```

---

## 📞 Поддержка

Если возникнут проблемы при деплое:
1. Проверьте логи на сервере
2. Убедитесь, что все переменные окружения настроены
3. Проверьте подключение к базе данных
4. Проверьте CORS настройки
5. Убедитесь, что порты открыты в firewall

