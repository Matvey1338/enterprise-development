using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Enums;
using DryCleaning.Domain.Seeds;
using DryCleaning.Infrastructure.InMemory;

namespace DryCleaning.Tests;

/// <summary>
/// Проверки хранилища в памяти: идентификаторы, целостность ссылок
/// между сущностями при обновлении и аналитические запросы
/// </summary>
public class RepositoryTests
{
    private static readonly DateTime _referenceMoment = new(2026, 11, 1, 12, 0, 0);

    /// <summary>
    /// Создаёт набор данных, зафиксированный относительно одного момента времени
    /// </summary>
    private static DryCleaningData CreateData() => DryCleaningDataSeeder.Create(_referenceMoment);

    /// <summary>
    /// Обновление категории сохраняет ссылки в изделиях и заказах:
    /// заказы продолжают указывать на тот же объект категории с новыми значениями
    /// </summary>
    [Fact]
    public async Task UpdatingCategoryShouldPreserveReferencesInOrders()
    {
        // arrange
        var data = CreateData();
        var categories = new InMemoryCategoryRepository(data.Categories);
        var orders = new InMemoryOrderRepository(data.Orders);
        var category = data.Categories[0];
        var clientOfOrder0 = data.Orders[0].Client; // Иванов: заказы 0, 1, 2, 3, 20

        // act
        var updated = await categories.UpdateAsync(category.CategoryId, new Category
        {
            CategoryId = 0,
            Name = "Верхняя одежда премиум",
            RecommendedCleaningType = CleaningType.Wet,
            CleaningPrice = 2000m,
        });

        // assert: тот же объект категории виден из заказа с новыми значениями
        Assert.True(updated);
        Assert.Same(category, await categories.GetByIdAsync(category.CategoryId));
        var order = data.Orders.First(o => o.Item.Category.CategoryId == 0);
        Assert.Same(category, order.Item.Category);
        Assert.Equal("Верхняя одежда премиум", order.Item.Category.Name);
        Assert.Equal(2000m, order.Item.Category.CleaningPrice);

        // сумма трат клиента пересчитывается по новой цене: 4750 + 2 × (2000 - 1500) = 5750
        var spending = await orders.GetClientSpendingAsync();
        Assert.Equal(5750m, spending[clientOfOrder0.ClientId]);
    }

    /// <summary>
    /// Обновление клиента не оставляет устаревший объект в заказах
    /// и не влияет на результаты запроса топ-клиентов
    /// </summary>
    [Fact]
    public async Task UpdatingClientShouldPreserveReferencesInOrders()
    {
        // arrange
        var data = CreateData();
        var clients = new InMemoryClientRepository(data.Clients);
        var orders = new InMemoryOrderRepository(data.Orders);
        var client = data.Clients[1]; // Петров: заказы 4, 5, 6, 15

        // act
        var updated = await clients.UpdateAsync(client.ClientId, new Client
        {
            ClientId = 1,
            FullName = "Петров Пётр Петрович-старший",
            Phone = client.Phone,
        });

        // assert
        Assert.True(updated);
        Assert.Same(client, await clients.GetByIdAsync(client.ClientId));
        Assert.All(data.Orders.Where(o => o.Client.ClientId == 1), o =>
        {
            Assert.Same(client, o.Client);
            Assert.Equal("Петров Пётр Петрович-старший", o.Client.FullName);
        });

        // в топ-5 клиентов за период клиент входит один раз, с новым ФИО, на прежней позиции
        var today = DateOnly.FromDateTime(_referenceMoment);
        var top = await orders.GetTopClientsByItemsCountAsync(today.AddDays(-100), today.AddDays(-41), 5);
        Assert.Equal(5, top.Count);
        Assert.Equal("Петров Пётр Петрович-старший", top[1].Client.FullName);
        Assert.Equal(3, top[1].ItemsCount);
    }

    /// <summary>
    /// Обновление изделия сохраняет ссылку на него из заказа
    /// </summary>
    [Fact]
    public async Task UpdatingItemShouldPreserveReferenceInOrder()
    {
        // arrange
        var data = CreateData();
        var items = new InMemoryItemRepository(data.Items);
        var item = data.Items[17]; // Шуба из заказа 17

        // act
        var updated = await items.UpdateAsync(item.ItemId, new Item
        {
            ItemId = 17,
            Name = "Шуба норковая",
            Material = item.Material,
            Category = item.Category,
        });

        // assert
        Assert.True(updated);
        var order = data.Orders.Single(o => o.OrderId == 17);
        Assert.Same(item, order.Item);
        Assert.Equal("Шуба норковая", order.Item.Name);
        Assert.True(await items.HasCategoryAsync(item.Category.CategoryId));
    }

    /// <summary>
    /// При добавлении выдаётся следующий свободный идентификатор;
    /// пустое хранилище нумерует с нуля
    /// </summary>
    [Fact]
    public async Task AddShouldAssignNextIdentifier()
    {
        // arrange
        var data = CreateData();
        var orders = new InMemoryOrderRepository(data.Orders);
        var clients = new InMemoryClientRepository(data.Clients);
        var emptyOrders = new InMemoryOrderRepository([]);

        // act
        var addedOrder = await orders.AddAsync(new Order
        {
            OrderId = 999,
            Client = data.Clients[0],
            Item = data.Items[0],
            ReceiptDate = DateOnly.FromDateTime(_referenceMoment),
            CompletionTermDays = 3,
            Status = OrderStatus.Accepted,
        });
        var addedClient = await clients.AddAsync(new Client
        {
            ClientId = 999,
            FullName = "Тестов Тест Тестович",
            Phone = "+7 (999) 999-99-99",
        });
        var addedToEmpty = await emptyOrders.AddAsync(new Order
        {
            OrderId = 777,
            Client = data.Clients[0],
            Item = data.Items[0],
            ReceiptDate = DateOnly.FromDateTime(_referenceMoment),
            CompletionTermDays = 3,
            Status = OrderStatus.Accepted,
        });

        // assert
        Assert.Equal(28, addedOrder.OrderId); // максимальный сид — 27
        Assert.Equal(12, addedClient.ClientId); // максимальный сид — 11
        Assert.Equal(0, addedToEmpty.OrderId);
        Assert.Equal(29, (await orders.GetAllAsync()).Count);
    }

    /// <summary>
    /// Удаление заказа отражается в аналитических запросах и проверках ссылок
    /// </summary>
    [Fact]
    public async Task DeletingOrderShouldAffectAnalyticsAndReferences()
    {
        // arrange
        var data = CreateData();
        var orders = new InMemoryOrderRepository(data.Orders);
        var today = DateOnly.FromDateTime(_referenceMoment);

        // act: удаляем заказ 18 (в обработке, срок 14 дней, клиент Соловьёв)
        var deleted = await orders.DeleteAsync(18);
        var deletedAgain = await orders.DeleteAsync(18);

        // assert
        Assert.True(deleted);
        Assert.False(deletedAgain);
        Assert.Null(await orders.GetByIdAsync(18));

        // в обработке остались заказы 15, 16, 17 — по возрастанию даты приёма
        var inProcessing = await orders.GetInProcessingAsync();
        Assert.Equal([17, 16, 15], inProcessing.Select(o => o.OrderId).ToArray());

        // максимальный срок 14 дней остался у заказов 17 и 22 — только Морозова
        var longest = await orders.GetClientsWithLongestProcessingAsync();
        var longestClient = Assert.Single(longest);
        Assert.Equal("Морозова Ольга Ивановна", longestClient.FullName);

        // ссылки на клиента 11 и изделие 18 больше не существуют
        Assert.False(await orders.HasClientAsync(11));
        Assert.False(await orders.HasItemAsync(18));
        Assert.True(await orders.HasClientAsync(6)); // Морозова — заказы 17 и 22
    }

    /// <summary>
    /// Аналитический запрос корректно работает на пустом хранилище
    /// </summary>
    [Fact]
    public async Task LongestProcessingOnEmptyRepositoryShouldReturnEmpty()
    {
        // arrange
        var orders = new InMemoryOrderRepository([]);

        // act
        var longest = await orders.GetClientsWithLongestProcessingAsync();
        var inProcessing = await orders.GetInProcessingAsync();
        var spending = await orders.GetClientSpendingAsync();

        // assert
        Assert.Empty(longest);
        Assert.Empty(inProcessing);
        Assert.Empty(spending);
    }
}
