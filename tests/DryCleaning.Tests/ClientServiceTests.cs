using DryCleaning.Application.Exceptions;
using DryCleaning.Contracts.Dto;

namespace DryCleaning.Tests;

/// <summary>
/// CRUD-операции сервиса клиентов: присвоение идентификаторов,
/// обновление с сохранением ссылок в заказах
/// и защита клиента с заказами от удаления
/// </summary>
public sealed class ClientServiceTests
{
    private readonly ServiceSet _services = ServiceSetFactory.Create();

    [Fact]
    public async Task AddClient_AssignsNextIdentifierAndReturnsDto()
    {
        // Arrange: эталон - максимальный занятый идентификатор 11, новый получит 12
        var upsert = new UpsertClientDto
        {
            FullName = "Козлова Дарья Андреевна",
            Phone = "+7 (934) 567-89-10",
        };

        // Act
        var added = await _services.Clients.AddClientAsync(upsert);

        // Assert
        Assert.Equal(12, added.ClientId);
        Assert.Equal("Козлова Дарья Андреевна", added.FullName);
        Assert.Equal("+7 (934) 567-89-10", added.Phone);
    }

    [Fact]
    public async Task UpdateClient_ExistingId_ReflectsInOrders()
    {
        // Arrange: заказ 0 оформлен клиентом 0 (Иванов);
        // переименование клиента должно отразиться в DTO заказа
        const int orderId = 0;
        var upsert = new UpsertClientDto
        {
            FullName = "Иванов Иван Иванович-младший",
            Phone = "+7 (912) 345-67-89",
        };

        // Act
        await _services.Clients.UpdateClientAsync(0, upsert);

        // Assert
        var order = await _services.Orders.GetOrderAsync(orderId);
        Assert.Equal("Иванов Иван Иванович-младший", order?.ClientFullName);
    }

    [Fact]
    public async Task DeleteClient_WithOrders_ThrowsEntityInUseException()
    {
        // Arrange: за клиентом 0 (Иванов) числятся заказы
        const int clientIdInUse = 0;

        // Act + Assert: клиент остаётся в списке
        await Assert.ThrowsAsync<EntityInUseException>(
            () => _services.Clients.DeleteClientAsync(clientIdInUse));
        Assert.NotNull(await _services.Clients.GetClientAsync(clientIdInUse));
    }
}
