# Многоступенчатый Dockerfile для ASP.NET Core приложения

# Этап 1: Сборка
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
COPY ["Self_care.csproj", "./"]
RUN dotnet restore "Self_care.csproj"

# Копируем весь код и собираем приложение
COPY . .
WORKDIR "/src"
RUN dotnet build "Self_care.csproj" -c Release -o /app/build

# Этап 2: Публикация
FROM build AS publish
RUN dotnet publish "Self_care.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Копируем миграции в опубликованную папку (если они есть)
RUN if [ -d "Migrations" ]; then cp -r Migrations /app/publish/; fi

# Этап 3: Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Создаем пользователя для безопасности
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Копируем опубликованное приложение
COPY --from=publish /app/publish .

# Устанавливаем права доступа
RUN chown -R appuser:appuser /app

# Переключаемся на непривилегированного пользователя
USER appuser

# Открываем порт
EXPOSE 8080

# Устанавливаем переменные окружения
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Запускаем приложение
ENTRYPOINT ["dotnet", "Self_care.dll"]

