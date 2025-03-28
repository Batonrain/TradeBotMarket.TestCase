# Arbitrage Service

Сервис для мониторинга разницы цен между квартальными фьючерсами на Binance. Проект реализован на основе микросервисной архитектуры с применением принципов чистой архитектуры и SOLID.

## Требования

- .NET 8.0 SDK
- PostgreSQL (или Docker)
- Docker (опционально)

## Структура проекта

- `ArbitrageService.Core` - Доменная модель и бизнес-логика
  - Модели данных (FuturesPrice, PriceDifference)
  - Интерфейсы репозиториев и сервисов
  - Сервисы бизнес-логики
- `ArbitrageService.Infrastructure` - Реализация инфраструктурного слоя
  - Реализация репозиториев
  - Реализация сервиса Binance API
  - Конфигурация базы данных
- `ArbitrageService.Api` - REST API сервис
  - REST endpoints для получения данных
  - Swagger документация
  - Логирование через Serilog
- `ArbitrageService.Worker` - Фоновый сервис
  - Периодический сбор данных через Quartz.NET
  - Расчет разницы цен
  - Сохранение данных в базу

## Быстрый старт

1. Запустите PostgreSQL через Docker:
```bash
docker run --name arbitrage-postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:latest
```

2. Примените миграции базы данных:
```bash
dotnet ef database update --project ArbitrageService.Infrastructure --startup-project ArbitrageService.Api
```

3. Запустите API сервис:
```bash
cd ArbitrageService.Api
dotnet run
```

4. В другом терминале запустите Worker сервис:
```bash
cd ArbitrageService.Worker
dotnet run
```

## API Endpoints

### Получение последней разницы цен
```http
GET /api/PriceDifference/latest
```

Параметры запроса:
- `firstSymbol` (string, required) - Символ первого фьючерса (например, BTCUSDT)
- `secondSymbol` (string, required) - Символ второго фьючерса (например, BTCUSDT_240628)

Пример ответа:
```json
{
  "id": "guid",
  "firstSymbol": "BTCUSDT",
  "secondSymbol": "BTCUSDT_240628",
  "firstPrice": 50000.00000000,
  "secondPrice": 50100.00000000,
  "difference": -100.00000000,
  "timestamp": "2024-03-28T12:00:00Z",
  "createdAt": "2024-03-28T12:00:00Z"
}
```

### Получение разницы цен за период
```http
GET /api/PriceDifference/range
```

Параметры запроса:
- `firstSymbol` (string, required) - Символ первого фьючерса
- `secondSymbol` (string, required) - Символ второго фьючерса
- `startTime` (datetime, required) - Начало периода в формате ISO 8601
- `endTime` (datetime, required) - Конец периода в формате ISO 8601

## Конфигурация

### База данных
Строка подключения настраивается в файлах:
- `ArbitrageService.Api/appsettings.json`
- `ArbitrageService.Worker/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ArbitrageService;Username=postgres;Password=postgres"
  }
}
```

### Логирование
Логи сохраняются в:
- Консоль (все сервисы)
- Файлы в директории `logs` с ежедневной ротацией

### Worker
- Периодичность сбора данных: каждую минуту (настраивается через CRON выражение в `Program.cs`)
- Автоматический фоллбек на последнюю известную цену при недоступности API

## Архитектура

Проект реализован с использованием следующих паттернов и принципов:

- Clean Architecture
  - Независимая доменная модель
  - Инверсия зависимостей
  - Разделение на слои
- SOLID принципы
  - Single Responsibility Principle (каждый класс имеет одну ответственность)
  - Open/Closed Principle (расширяемость через интерфейсы)
  - Liskov Substitution Principle (корректное использование наследования)
  - Interface Segregation Principle (специализированные интерфейсы)
  - Dependency Inversion Principle (зависимости от абстракций)
- Repository Pattern для работы с данными
- Dependency Injection для управления зависимостями
- Микросервисная архитектура (API и Worker сервисы)

## Технологии

- ASP.NET Core 8.0
- Entity Framework Core 8.0
- PostgreSQL
- Quartz.NET 3.8.0 (планировщик задач)
- Serilog (структурированное логирование)
- Swagger/OpenAPI (документация API)
- Docker (контейнеризация) 