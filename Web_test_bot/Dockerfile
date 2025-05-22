# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем только файлы проекта (для оптимизации кэша Docker)
COPY *.csproj .
RUN dotnet restore

# Копируем все остальные файлы
COPY . .

# Собираем и публикуем проект
RUN dotnet publish -c Release -o /app/publish

# Этап рантайма
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80
# Копируем собранное приложение
COPY --from=build /app/publish .

# Копируем базу данных и JSON-файлы
COPY MyDatabase2.db .
COPY appsettings.json .
COPY botInst.json .

# Даем права на запись в базу данных (важно для Linux)
RUN chmod 666 MyDatabase2.db

# Запускаем приложение
ENTRYPOINT ["dotnet", "Web_test_bot.dll"]