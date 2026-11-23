using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Tests;

/// <summary>
/// CRUD-операции сервиса заказов: присвоение идентификаторов
/// с человекочитаемыми именами в DTO, проверка существования клиента
/// и удаление заказа без ограничений
/// </summary>
public sealed class OrderServiceTests
{
    private readonly ServiceSet _services = ServiceSetFactory.Create();

    [Fact]
    public async Task AddOrder_AssignsNextIdentifierAndReturnsDtoWithNames()
    {
        // Arrange: эталон - 28 заказов (идентификаторы 0-27), новый получит 28;
        // клиент 2 - Сидорова, изделие 5 - пиджак Петрова
        var upsert = new UpsertOrderDto
        {
            ClientId = 2,
            ItemId = 5,
            ReceiptDate = _services.ReferenceDate,
            CompletionTermDays = 7,
            Status = OrderStatus.Accepted,
        };

        // Act
        var added = await _services.Orders.AddOrderAsync(upsert);

        // Assert: DTO содержит человекочитаемые имена клиента и изделия
        Assert.Equal(28, added.OrderId);
        Assert.Equal(2, added.ClientId);
        Assert.Equal("Сидорова Анна Сергеевна", added.ClientFullName);
        Assert.Equal(5, added.ItemId);
        Assert.Equal("Пиджак", added.ItemName);
        Assert.Equal(OrderStatus.Accepted, added.Status);
    }

    [Fact]
    public async Task AddOrder_MissingClient_ThrowsValidationException()
    {
        // Arrange: клиента с идентификатором 999 не существует
        var upsert = new UpsertOrderDto
        {
            ClientId = 999,
            ItemId = 5,
            ReceiptDate = _services.ReferenceDate,
            CompletionTermDays = 7,
            Status = OrderStatus.Accepted,
        };

        // Act + Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _services.Orders.AddOrderAsync(upsert));
    }

    [Fact]
    public async Task DeleteOrder_ExistingId_ReturnsTrue()
    {
        // Arrange: заказы ни на что не ссылаются извне, поэтому удаляются свободно;
        // удаляем отменённый заказ 25 (ковёр Фёдорова)
        const int orderId = 25;

        // Act
        var deleted = await _services.Orders.DeleteOrderAsync(orderId);

        // Assert
        Assert.True(deleted);
        Assert.Null(await _services.Orders.GetOrderAsync(orderId));
    }
}
