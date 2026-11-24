using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using DryCleaning.Api.Controllers;
using DryCleaning.Contracts.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DryCleaning.Tests;

/// <summary>
/// Проверки API через HTTP: CRUD, валидация, ошибки, аналитика и документация
/// </summary>
public class ApiTests(WebApplicationFactory<ClientsController> factory)
    : IClassFixture<WebApplicationFactory<ClientsController>>
{
    private static readonly Dictionary<string, string> _payloads = new()
    {
        ["clients"] = """
            {"fullName":"Тестовый клиент","phone":"+79999999999"}
            """,
        ["categories"] = """
            {"name":"Тестовая категория","recommendedCleaningType":"Dry","cleaningPrice":999}
            """,
        ["items"] = """
            {"name":"Тестовое изделие","material":"Шерсть","categoryId":0}
            """,
        ["orders"] = """
            {"clientId":0,"itemId":0,"receiptDate":"2026-01-15",
             "completionTermDays":5,"status":"InProcessing"}
            """,
    };

    /// <summary>
    /// Идентификатор формируется сервером, Location доступен,
    /// PUT возвращает обновлённый DTO
    /// </summary>
    [Theory]
    [InlineData("clients", "clientId", "fullName", "Обновлённое значение")]
    [InlineData("categories", "categoryId", "name", "Обновлённое значение")]
    [InlineData("items", "itemId", "name", "Обновлённое значение")]
    [InlineData("orders", "orderId", "status", "Completed")]
    public async Task CrudShouldReturnExpectedStatusesAndPreserveIdentifier(
        string resource, string idField, string changedField, string changedValue)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = factory.CreateClient();
        var payload = JsonNode.Parse(_payloads[resource])!.AsObject();
        var created = await client.PostAsJsonAsync($"/api/{resource}", payload, cancellationToken);
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var dto = (await created.Content.ReadFromJsonAsync<JsonObject>(cancellationToken))!;
        var id = dto[idField]!.GetValue<int>();
        Assert.True(id >= 10);
        Assert.NotNull(created.Headers.Location);
        var fetched = await client.GetFromJsonAsync<JsonObject>(created.Headers.Location, cancellationToken);
        Assert.Equal(id, fetched![idField]!.GetValue<int>());

        payload[changedField] = changedValue;
        var updated = await client.PutAsJsonAsync($"/api/{resource}/{id}", payload, cancellationToken);
        Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
        var updatedDto = (await updated.Content.ReadFromJsonAsync<JsonObject>(cancellationToken))!;
        Assert.Equal(id, updatedDto[idField]!.GetValue<int>());
        Assert.Equal(changedValue, updatedDto[changedField]!.GetValue<string>());
        fetched = await client.GetFromJsonAsync<JsonObject>($"/api/{resource}/{id}", cancellationToken);
        Assert.Equal(changedValue, fetched![changedField]!.GetValue<string>());

        Assert.Equal(HttpStatusCode.NoContent,
            (await client.DeleteAsync($"/api/{resource}/{id}", cancellationToken)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await client.GetAsync($"/api/{resource}/{id}", cancellationToken)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await client.DeleteAsync($"/api/{resource}/{id}", cancellationToken)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound,
            (await client.PutAsJsonAsync($"/api/{resource}/{id}", payload, cancellationToken)).StatusCode);
    }

    /// <summary>
    /// Отсутствующие обязательные поля не подменяются значениями по умолчанию
    /// </summary>
    [Theory]
    [InlineData("clients", "fullName")]
    [InlineData("clients", "phone")]
    [InlineData("categories", "name")]
    [InlineData("categories", "recommendedCleaningType")]
    [InlineData("categories", "cleaningPrice")]
    [InlineData("items", "name")]
    [InlineData("items", "material")]
    [InlineData("items", "categoryId")]
    [InlineData("orders", "clientId")]
    [InlineData("orders", "itemId")]
    [InlineData("orders", "receiptDate")]
    [InlineData("orders", "completionTermDays")]
    [InlineData("orders", "status")]
    public async Task MissingRequiredFieldsShouldReturnValidationProblem(string resource, string field)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = factory.CreateClient();
        var payload = JsonNode.Parse(_payloads[resource])!.AsObject();
        payload.Remove(field);
        var response = await client.PostAsJsonAsync($"/api/{resource}", payload, cancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken);
        Assert.NotEmpty(problem!.Errors);
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.PutAsJsonAsync($"/api/{resource}/0", payload, cancellationToken)).StatusCode);
    }

    /// <summary>
    /// Некорректные телефоны, диапазоны, статусы и ссылки на несуществующие
    /// сущности отклоняются и не меняют сохранённые данные
    /// </summary>
    [Theory]
    [InlineData("clients", "phone", "\"invalid\"")]
    [InlineData("categories", "cleaningPrice", "0")]
    [InlineData("categories", "cleaningPrice", "-50")]
    [InlineData("categories", "recommendedCleaningType", "0")]
    [InlineData("items", "categoryId", "-1")]
    [InlineData("items", "categoryId", "999999")]
    [InlineData("orders", "clientId", "-1")]
    [InlineData("orders", "clientId", "999999")]
    [InlineData("orders", "itemId", "999999")]
    [InlineData("orders", "completionTermDays", "0")]
    [InlineData("orders", "completionTermDays", "366")]
    [InlineData("orders", "status", "99")]
    public async Task InvalidInputShouldNotChangeStoredData(string resource, string field, string value)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = factory.CreateClient();
        var before = await client.GetStringAsync($"/api/{resource}", cancellationToken);
        var payload = JsonNode.Parse(_payloads[resource])!.AsObject();
        payload[field] = JsonNode.Parse(value);
        var response = await client.PostAsJsonAsync($"/api/{resource}", payload, cancellationToken);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>(cancellationToken);
        Assert.NotEmpty(problem!.Errors);
        Assert.Equal(HttpStatusCode.BadRequest,
            (await client.PutAsJsonAsync($"/api/{resource}/0", payload, cancellationToken)).StatusCode);
        Assert.Equal(before, await client.GetStringAsync($"/api/{resource}", cancellationToken));
    }

    /// <summary>
    /// Связанные записи нельзя удалить, пока на них ссылаются другие сущности
    /// </summary>
    [Theory]
    [InlineData("categories")]
    [InlineData("clients")]
    [InlineData("items")]
    public async Task DeletingReferencedEntityShouldReturnConflict(string resource)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.Conflict,
            (await client.DeleteAsync($"/api/{resource}/0", cancellationToken)).StatusCode);
        Assert.Equal(HttpStatusCode.OK,
            (await client.GetAsync($"/api/{resource}/0", cancellationToken)).StatusCode);
    }

    /// <summary>
    /// Некорректные параметры аналитических запросов отклоняются до обращения к данным
    /// </summary>
    [Theory]
    [InlineData("/api/categories/most-popular?count=0")]
    [InlineData("/api/categories/least-popular?count=0")]
    [InlineData("/api/categories/most-popular?count=abc")]
    [InlineData("/api/clients/top-by-items?count=0")]
    [InlineData("/api/clients/top-by-items?from=invalid")]
    public async Task InvalidQueryShouldReturnBadRequest(string url)
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = factory.CreateClient();
        Assert.Equal(HttpStatusCode.BadRequest, (await client.GetAsync(url, cancellationToken)).StatusCode);
    }

    /// <summary>
    /// Аналитические запросы возвращают эталонные значения тестового набора данных
    /// </summary>
    [Fact]
    public async Task AnalyticsEndpointsShouldReturnSeedReferenceResults()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = factory.CreateClient();

        var topSpender = await client.GetFromJsonAsync<JsonObject>("/api/clients/top-spender", cancellationToken);
        Assert.Equal(1, topSpender!["clientId"]!.GetValue<int>());
        Assert.Equal("Петров Пётр Петрович", topSpender["fullName"]!.GetValue<string>());
        Assert.Equal(6450m, topSpender["totalSpent"]!.GetValue<decimal>());

        var inProcessing = Assert.IsType<JsonArray>(
            await client.GetFromJsonAsync<JsonArray>(
                "/api/orders/in-processing", cancellationToken));

        Assert.Equal([18, 17, 16, 15],
            inProcessing.Select(o => o!["orderId"]!.GetValue<int>()).ToArray());

        Assert.All(inProcessing,
            o => Assert.Equal("InProcessing", o!["status"]!.GetValue<string>()));

        var mostPopular = await client.GetFromJsonAsync<JsonArray>("/api/categories/most-popular", cancellationToken);
        Assert.Equal(5, mostPopular!.Count);
        Assert.Equal("Верхняя одежда", mostPopular[0]!["name"]!.GetValue<string>());
        Assert.Equal(6, mostPopular[0]!["ordersCount"]!.GetValue<int>());

        var leastPopular = await client.GetFromJsonAsync<JsonArray>("/api/categories/least-popular", cancellationToken);
        Assert.Equal(5, leastPopular!.Count);
        Assert.Equal("Одеяла", leastPopular[0]!["name"]!.GetValue<string>());
        Assert.Equal(0, leastPopular[0]!["ordersCount"]!.GetValue<int>());

        var longestProcessing = await client.GetFromJsonAsync<JsonArray>(
            "/api/clients/longest-processing", cancellationToken);
        Assert.Equal([6, 11],
            longestProcessing!.Select(c => c!["clientId"]!.GetValue<int>()).ToArray());

        var from = DateOnly.FromDateTime(DateTime.Now).AddDays(-150).ToString("yyyy-MM-dd");
        var to = DateOnly.FromDateTime(DateTime.Now).AddDays(-40).ToString("yyyy-MM-dd");
        var topByItems = await client.GetFromJsonAsync<JsonArray>(
            $"/api/clients/top-by-items?from={from}&to={to}&count=5", cancellationToken);
        Assert.Equal(
            [(0, 4), (1, 3), (2, 3), (3, 2), (4, 2)],
            topByItems!.Select(c => (c!["clientId"]!.GetValue<int>(), c["itemsCount"]!.GetValue<int>())).ToArray());
    }

    /// <summary>
    /// Непредвиденная ошибка возвращает 500 без внутренних подробностей
    /// </summary>
    [Fact]
    public async Task UnexpectedFailureShouldReturnSafeProblemDetails()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        await using var failingFactory = factory.WithWebHostBuilder(builder => builder
            .UseEnvironment("Production")
            .ConfigureServices(services =>
            {
                services.RemoveAll<IClientService>();
                services.AddScoped<IClientService>(_ => throw new InvalidOperationException("private database details"));
            }));
        using var client = failingFactory.CreateClient();
        var response = await client.GetAsync("/api/clients", cancellationToken);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        Assert.DoesNotContain("private database details", body);
        Assert.DoesNotContain("InvalidOperationException", body);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(cancellationToken);
        Assert.Equal(500, problem!.Status);
        Assert.NotNull(problem.Extensions["traceId"]);
    }

    /// <summary>
    /// Scalar и OpenAPI доступны; схемы запросов не содержат серверных полей
    /// </summary>
    [Fact]
    public async Task OpenApiShouldDescribeSeparateRequestSchemasAndResponseStatuses()
    {
        var cancellationToken = TestContext.Current.CancellationToken;
        using var client = factory.CreateClient();
        Assert.Contains("scalar",
            await client.GetStringAsync("/scalar/v1", cancellationToken), StringComparison.OrdinalIgnoreCase);
        var document = (await client.GetFromJsonAsync<JsonObject>("/openapi/v1.json", cancellationToken))!;
        var schemas = document["components"]!["schemas"]!.AsObject();

        var serverFields = new Dictionary<string, (string IdField, string[] ComputedFields)>
        {
            ["Category"] = ("categoryId", []),
            ["Client"] = ("clientId", []),
            ["Item"] = ("itemId", ["categoryName"]),
            ["Order"] = ("orderId", ["clientFullName", "itemName"]),
        };
        foreach (var (entity, (idField, computedFields)) in serverFields)
        {
            var upsertProperties = schemas[$"Upsert{entity}Dto"]!["properties"]!.AsObject();
            Assert.False(upsertProperties.ContainsKey(idField));
            foreach (var computedField in computedFields)
            {
                Assert.False(upsertProperties.ContainsKey(computedField));
            }

            var dtoProperties = schemas[$"{entity}Dto"]!["properties"]!.AsObject();
            Assert.True(dtoProperties.ContainsKey(idField));
            foreach (var computedField in computedFields)
            {
                Assert.True(dtoProperties.ContainsKey(computedField));
            }
        }

        foreach (var path in document["paths"]!.AsObject())
        {
            foreach (var operation in path.Value!.AsObject())
            {
                Assert.True(operation.Value!["responses"]!.AsObject().ContainsKey("500"));
            }
        }
    }
}
