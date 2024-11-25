# Використовуємо базовий образ для ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Використовуємо SDK для побудови додатку
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Копіюємо файл проекту та виконуємо відновлення залежностей
COPY ["DailyOrganizer/DailyOrganizer.csproj", "DailyOrganizer/"]
RUN dotnet restore "DailyOrganizer/DailyOrganizer.csproj"

# Копіюємо всі файли та будуємо додаток
COPY . .
WORKDIR "/src/DailyOrganizer"
RUN dotnet build "DailyOrganizer.csproj" -c Release -o /app/build

# Публікуємо додаток
FROM build AS publish
RUN dotnet publish "DailyOrganizer.csproj" -c Release -o /app/publish

# Створюємо фінальний образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DailyOrganizer.dll"]