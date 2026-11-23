using DryCleaning.Application.Services;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain.Seeds;
using DryCleaning.Infrastructure.InMemory;

namespace DryCleaning.Tests;

/// <summary>
/// Набор сервисов поверх in-memory хранилищ, заполненных генератором данных.
/// Каждый тест создаёт собственный набор, поэтому изменения данных
/// в одном тесте не влияют на другие
/// </summary>
public sealed record ServiceSet(
    DateTime Now,
    ICategoryService Categories,
    IClientService Clients,
    IItemService Items,
    IOrderService Orders)
{
    /// <summary>
    /// Дата, относительно которой построены тестовые данные
    /// </summary>
    public DateOnly ReferenceDate => DateOnly.FromDateTime(Now);
}

/// <summary>
/// Создаёт наборы сервисов с данными генератора и синхронизированными часами:
/// сервисы, которым нужен текущий момент, получают тот же момент,
/// относительно которого построены данные
/// </summary>
public static class ServiceSetFactory
{
    /// <summary>
    /// Создаёт набор сервисов с данными относительно указанного момента;
    /// по умолчанию — текущий момент
    /// </summary>
    /// <param name="now">Момент времени, относительно которого построены данные</param>
    public static ServiceSet Create(DateTime? now = null)
    {
        var moment = now ?? DateTime.Now;
        var data = DryCleaningDataSeeder.Create(moment);

        var categoryRepository = new InMemoryCategoryRepository(data.Categories);
        var clientRepository = new InMemoryClientRepository(data.Clients);
        var itemRepository = new InMemoryItemRepository(data.Items);
        var orderRepository = new InMemoryOrderRepository(data.Orders);

        return new ServiceSet(
            moment,
            new CategoryService(categoryRepository, itemRepository, orderRepository, () => moment),
            new ClientService(clientRepository, orderRepository),
            new ItemService(itemRepository, categoryRepository, orderRepository),
            new OrderService(orderRepository, clientRepository, itemRepository));
    }
}
