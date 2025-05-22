FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем проект в подпапку
COPY Web_test_bot/*.csproj Web_test_bot/
RUN dotnet restore Web_test_bot/Web_test_bot.csproj

# Копируем остальной код
COPY . .

# Публикуем из нужной папки
RUN dotnet publish Web_test_bot/Web_test_bot.csproj -c Release -o /app/publish

# Финальный образ
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

# Копируем конфиги и БД
COPY Web_test_bot/MyDatabase2.db .
COPY Web_test_bot/appsettings.json .
COPY Web_test_bot/botInst.json .

RUN chmod 666 MyDatabase2.db

ENTRYPOINT ["dotnet", "Web_test_bot.dll"]
