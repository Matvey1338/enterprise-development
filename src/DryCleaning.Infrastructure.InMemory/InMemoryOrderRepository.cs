using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Enums;
using DryCleaning.Domain.Interfaces;
using DryCleaning.Domain.Models;

namespace DryCleaning.Infrastructure.InMemory;

/// <summary>
/// Репозиторий заказов с хранением в памяти
/// Выполняет аналитические запросы над коллекцией заказов;
/// </summary>
/// <param name="seed">Начальные данные.</param>
public class InMemoryOrderRepository(IReadOnlyList<Order> seed)
    : InMemoryRepository<Order>(seed), IOrderRepository
{
    /// <inheritdoc/>
    public Task<IReadOnlyList<Order>> GetInProcessingAsync() =>
        ReadAsync<IReadOnlyList<Order>>(items =>
            [.. items.Where(order => order.Status == OrderStatus.InProcessing)
                .OrderBy(order => order.ReceiptDate)]);

    /// <inheritdoc/>
    public Task<IReadOnlyList<ClientItemsCount>> GetTopClientsByItemsCountAsync(DateOnly from, DateOnly to, int count) =>
        ReadAsync<IReadOnlyList<ClientItemsCount>>(items =>
            [.. items
                .Where(order => order.ReceiptDate >= from && order.ReceiptDate <= to)
                .GroupBy(order => order.Client.ClientId)
                .Select(group => new ClientItemsCount(group.First().Client, group.Count()))
                .OrderByDescending(result => result.ItemsCount)
                .ThenBy(result => result.Client.FullName, StringComparer.Ordinal)
                .Take(count)]);

    /// <inheritdoc/>
    public Task<IReadOnlyList<Client>> GetClientsWithLongestProcessingAsync() =>
        ReadAsync<IReadOnlyList<Client>>(items =>
        {
            if (items.Count == 0)
            {
                return [];
            }

            var maxTermDays = items.Max(order => order.CompletionTermDays);
            return [.. items
                .Where(order => order.CompletionTermDays == maxTermDays)
                .Select(order => order.Client)
                .DistinctBy(client => client.ClientId)
                .OrderBy(client => client.FullName, StringComparer.Ordinal)];
        });

    /// <inheritdoc/>
    public Task<IReadOnlyDictionary<int, int>> GetCategoryOrdersCountsAsync(DateOnly since) =>
        ReadAsync<IReadOnlyDictionary<int, int>>(items =>
            items.Where(order => order.ReceiptDate >= since)
                .GroupBy(order => order.Item.Category.CategoryId)
                .ToDictionary(group => group.Key, group => group.Count()));

    /// <inheritdoc/>
    public Task<IReadOnlyDictionary<int, decimal>> GetClientSpendingAsync() =>
        ReadAsync<IReadOnlyDictionary<int, decimal>>(items =>
            items.Where(order => order.Status != OrderStatus.Cancelled)
                .GroupBy(order => order.Client.ClientId)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(order => order.Item.Category.CleaningPrice)));

    /// <inheritdoc/>
    public Task<bool> HasClientAsync(int clientId) =>
        ReadAsync(items => items.Any(order => order.Client.ClientId == clientId));

    /// <inheritdoc/>
    public Task<bool> HasItemAsync(int itemId) =>
        ReadAsync(items => items.Any(order => order.Item.ItemId == itemId));

    /// <inheritdoc/>
    protected override int GetId(Order entity) => entity.OrderId;

    /// <inheritdoc/>
    protected override void SetId(Order entity, int id) => entity.OrderId = id;

    /// <inheritdoc/>
    protected override void CopyProperties(Order source, Order target)
    {
        target.Client = source.Client;
        target.Item = source.Item;
        target.ReceiptDate = source.ReceiptDate;
        target.CompletionTermDays = source.CompletionTermDays;
        target.Status = source.Status;
    }
}
