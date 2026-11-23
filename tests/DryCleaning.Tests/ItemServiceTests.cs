using System.ComponentModel.DataAnnotations;
using DryCleaning.Application.Exceptions;
using DryCleaning.Contracts.Dto;

namespace DryCleaning.Tests;

/// <summary>
/// CRUD-операции сервиса изделий: проверка существования категории,
/// обновление с сохранением ссылок в заказах
/// и защита изделия с заказами от удаления
/// </summary>
public sealed class ItemServiceTests
{
    private readonly ServiceSet _services = ServiceSetFactory.Create();

    [Fact]
    public async Task AddItem_MissingCategory_ThrowsValidationException()
    {
        // Arrange: категории с идентификатором 999 не существует
        var upsert = new UpsertItemDto
        {
            Name = "Кепка",
            Material = "хлопок",
            CategoryId = 999,
        };

        // Act + Assert
        await Assert.ThrowsAsync<ValidationException>(
            () => _services.Items.AddItemAsync(upsert));
    }

    [Fact]
    public async Task UpdateItem_ExistingId_ReflectsInOrders()
    {
        // Arrange: изделие 0 (пальто) входит в заказ 0;
        // переименование изделия должно отразиться в DTO заказа
        var upsert = new UpsertItemDto
        {
            Name = "Пальто демисезонное",
            Material = "шерсть",
            CategoryId = 0,
        };

        // Act
        await _services.Items.UpdateItemAsync(0, upsert);

        // Assert
        var order = await _services.Orders.GetOrderAsync(0);
        Assert.Equal("Пальто демисезонное", order?.ItemName);
    }

    [Fact]
    public async Task DeleteItem_WithOrders_ThrowsEntityInUseException()
    {
        // Arrange: изделие 0 входит в заказ 0
        const int itemIdInUse = 0;

        // Act + Assert: изделие остаётся в списке
        await Assert.ThrowsAsync<EntityInUseException>(
            () => _services.Items.DeleteItemAsync(itemIdInUse));
        Assert.NotNull(await _services.Items.GetItemAsync(itemIdInUse));
    }
}
