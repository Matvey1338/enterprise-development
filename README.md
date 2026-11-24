# Курс «Разработка корпоративных приложений». Тема: «Серверное приложение» — REST API предметной области и его тестирование

| Параметр | Значение |
|---|---|
| Автор | Ухтверов Матвей |
| Группа | 6412 |
| Вариант | 58 |
| Предметная область | Химчистка |

## Реализация

Объектная модель лабораторной работы № 1 развёрнута в серверное приложение на ASP.NET Core. Решение построено по принципам «чистой» архитектуры: зависимости направлены только внутрь, к Domain.

| Проект | Назначение |
|---|---|
| `DryCleaning.Domain` | Сущности, перечисления и интерфейсы репозиториев; не зависит от других проектов |
| `DryCleaning.Contracts` | DTO и интерфейсы сервисов: контракт API, общий для сервера и его клиентов |
| `DryCleaning.Application` | Сервисы предметной области: CRUD, пять аналитических запросов, бизнес-правила |
| `DryCleaning.Infrastructure.InMemory` | Реализации репозиториев на коллекциях в памяти с потокобезопасным доступом |
| `DryCleaning.Api` | Контроллеры, регистрации зависимостей и обработчик ошибок; точка входа |
| `DryCleaning.Tests` | Модульные и интеграционные тесты (xunit.v3) |

Данные хранятся в памяти и засеиваются генератором `DryCleaningDataSeeder` при старте сервера: 10 категорий, 12 клиентов, 28 изделий и 28 заказов. Репозитории зарегистрированы в контейнере зависимостей как singleton, поэтому изменения переживают отдельные HTTP-запросы; сервисы — scoped и получают свои зависимости через конструктор. Интерфейсы репозиториев остаются точкой расширения: в следующих лабораторных работах под них подключаются MySQL и брокер сообщений NATS, предметная логика не меняется.

## API

CRUD-операции каждой сущности и пять аналитических запросов:

| Метод и маршрут | Назначение |
|---|---|
| `GET /api/categories`, `POST /api/categories`, `GET /api/categories/{id}`, `PUT /api/categories/{id}`, `DELETE /api/categories/{id}` | CRUD категорий изделий |
| `GET /api/clients`, `POST /api/clients`, `GET /api/clients/{id}`, `PUT /api/clients/{id}`, `DELETE /api/clients/{id}` | CRUD клиентов |
| `GET /api/items`, `POST /api/items`, `GET /api/items/{id}`, `PUT /api/items/{id}`, `DELETE /api/items/{id}` | CRUD принятых изделий |
| `GET /api/orders`, `POST /api/orders`, `GET /api/orders/{id}`, `PUT /api/orders/{id}`, `DELETE /api/orders/{id}` | CRUD заказов |
| `GET /api/orders/in-processing` | Заказы, находящиеся в обработке, по дате приёма |
| `GET /api/clients/top-by-items?from&to&count` | Топ клиентов по числу сданных изделий за период |
| `GET /api/clients/longest-processing` | Клиенты с максимальным сроком обработки заказов |
| `GET /api/clients/top-spender` | Клиент с наибольшей суммой трат |
| `GET /api/categories/most-popular?count`, `GET /api/categories/least-popular?count` | Топ наиболее и наименее популярных категорий за последний год |

Идентификатор созданной сущности присваивается сервером и возвращается в заголовке `Location`. Ошибки возвращаются в формате ProblemDetails: некорректные данные — 400, отсутствующая сущность — 404, удаление сущности, на которую ссылаются другие, — 409, непредвиденная ошибка — 500 без внутренних подробностей. Преобразование исключений выполняет `ApiExceptionHandler` (`IExceptionHandler`); валидация входных DTO — атрибутами `System.ComponentModel.DataAnnotations` и `IValidatableObject`, поэтому некорректный запрос отклоняется до обращения к предметной логике.

## Документация API

OpenAPI-документ генерируется автоматически и доступен по пути `/openapi/v1.json`. Интерактивный интерфейс Scalar открывается на `http://localhost:5080/scalar`: там перечислены все эндпоинты, схемы DTO с описанием полей и кодами ответов, а запросы можно отправлять прямо из браузера.

## Запуск

```bash
dotnet run --project src/DryCleaning.Api
```

Сервер стартует на `http://localhost:5080` (профиль `http` в `launchSettings.json`). Статусы заказов передаются строками, например `"InProcessing"`.

## Модульные тесты

Тестовые данные готовит генератор `DryCleaningDataSeeder`: даты приёма заказов вычисляются относительно даты, зафиксированной при создании набора, поэтому результаты не зависят от дня запуска. Сервисные тесты работают через фикстуру `ServiceSet` поверх тех же репозиториев, что и сервер, с часами, синхронизированными с данными. Каждый запрос покрыт отдельным тестовым классом:

| Запрос | Тестовый класс |
|---|---|
| Заказы, находящиеся в обработке, упорядоченные по дате приёма | `OrdersInProcessingTests` |
| Топ-5 клиентов, сдавших больше всего изделий за заданный период | `TopClientsByItemsInPeriodTests` |
| Клиенты, чьи заказы обрабатывались дольше всего, упорядоченные по ФИО | `ClientsWithLongestProcessingTests` |
| Топ-5 наиболее и наименее популярных категорий изделий за последний год | `CategoryPopularityTests` |
| Клиент, потративший наибольшую сумму за весь период работы химчистки | `TopSpenderClientTests` |

CRUD-правила сервисов покрывают `CategoryServiceTests`, `ClientServiceTests`, `ItemServiceTests`, `OrderServiceTests` (присвоение идентификатора, валидация, ссылки на несуществующие сущности, запрет удаления используемых записей), репозитории в памяти — `RepositoryTests`.

Интеграционные тесты `ApiTests` поднимают приложение через `WebApplicationFactory` и проверяют его по настоящему HTTP-контракту, без запущенного сервера: полный жизненный цикл CRUD (201 → 200 → 204 → 404), отклонение отсутствующих и некорректных полей без изменения хранилища, конфликт 409 при удалении связанных сущностей, безопасное сообщение ProblemDetails при 500, эталонные результаты всех пяти аналитических запросов и соответствие OpenAPI-документа контракту (серверные поля отсутствуют в схемах запросов, каждая операция объявляет ответ 500).

Принятые допущения:

- длительность обработки заказа определяется его сроком выполнения (`CompletionTermDays`), так как фактическая дата завершения в предметной области не хранится;
- категории без заказов за последний год считаются наименее популярными (0 заказов);
- отменённые заказы не оплачиваются и не учитываются в сумме трат клиентов.

## Структура решения

```text
enterprise-development/
├── .github/
│   └── workflows/
│       └── tests.yml
├── DryCleaning.slnx
├── README.md
├── src/
│   ├── DryCleaning.Api/
│   │   ├── Controllers/
│   │   │   ├── CategoriesController.cs
│   │   │   ├── ClientsController.cs
│   │   │   ├── ItemsController.cs
│   │   │   └── OrdersController.cs
│   │   ├── Extensions/
│   │   │   └── ServiceCollectionExtensions.cs
│   │   ├── Infrastructure/
│   │   │   └── ApiExceptionHandler.cs
│   │   ├── Properties/
│   │   │   └── launchSettings.json
│   │   ├── DryCleaning.Api.csproj
│   │   └── Program.cs
│   ├── DryCleaning.Application/
│   │   ├── Exceptions/
│   │   │   └── EntityInUseException.cs
│   │   ├── Services/
│   │   │   ├── CategoryService.cs
│   │   │   ├── ClientService.cs
│   │   │   ├── ItemService.cs
│   │   │   └── OrderService.cs
│   │   └── DryCleaning.Application.csproj
│   ├── DryCleaning.Contracts/
│   │   ├── Dto/
│   │   │   ├── CategoryDto.cs
│   │   │   ├── CategoryPopularityDto.cs
│   │   │   ├── ClientDto.cs
│   │   │   ├── ClientItemsCountDto.cs
│   │   │   ├── ClientSpendingDto.cs
│   │   │   ├── ItemDto.cs
│   │   │   ├── OrderDto.cs
│   │   │   ├── UpsertCategoryDto.cs
│   │   │   ├── UpsertClientDto.cs
│   │   │   ├── UpsertItemDto.cs
│   │   │   └── UpsertOrderDto.cs
│   │   ├── Interfaces/
│   │   │   ├── ICategoryService.cs
│   │   │   ├── IClientService.cs
│   │   │   ├── IItemService.cs
│   │   │   └── IOrderService.cs
│   │   └── DryCleaning.Contracts.csproj
│   ├── DryCleaning.Domain/
│   │   ├── Entities/
│   │   │   ├── Category.cs
│   │   │   ├── Client.cs
│   │   │   ├── Item.cs
│   │   │   └── Order.cs
│   │   ├── Enums/
│   │   │   ├── CleaningType.cs
│   │   │   └── OrderStatus.cs
│   │   ├── Interfaces/
│   │   │   ├── ICategoryRepository.cs
│   │   │   ├── IClientRepository.cs
│   │   │   ├── IItemRepository.cs
│   │   │   ├── IOrderRepository.cs
│   │   │   └── IRepository.cs
│   │   ├── Models/
│   │   │   └── ClientItemsCount.cs
│   │   ├── Seeds/
│   │   │   ├── DryCleaningData.cs
│   │   │   └── DryCleaningDataSeeder.cs
│   │   └── DryCleaning.Domain.csproj
│   └── DryCleaning.Infrastructure.InMemory/
│       ├── InMemoryCategoryRepository.cs
│       ├── InMemoryClientRepository.cs
│       ├── InMemoryItemRepository.cs
│       ├── InMemoryOrderRepository.cs
│       ├── InMemoryRepository.cs
│       └── DryCleaning.Infrastructure.InMemory.csproj
└── tests/
    └── DryCleaning.Tests/
        ├── ApiTests.cs
        ├── CategoryPopularityTests.cs
        ├── CategoryServiceTests.cs
        ├── ClientServiceTests.cs
        ├── ClientsWithLongestProcessingTests.cs
        ├── DryCleaningFixture.cs
        ├── DryCleaning.Tests.csproj
        ├── ItemServiceTests.cs
        ├── OrderServiceTests.cs
        ├── OrdersInProcessingTests.cs
        ├── RepositoryTests.cs
        ├── ServiceSet.cs
        ├── TopClientsByItemsInPeriodTests.cs
        ├── TopSpenderClientTests.cs
        └── xunit.runner.json
```

## Сборка и запуск тестов

```bash
dotnet test
```
