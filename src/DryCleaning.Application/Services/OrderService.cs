using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Application.Services;

/// <summary>
/// Операции с заказами и преобразование данных в DTO
/// </summary>
public class OrderService(
    IOrderRepository orderRepository,
    IClientRepository clientRepository,
    IItemRepository itemRepository) : IOrderService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync() =>
        [.. (await orderRepository.GetAllAsync()).Select(MapToDto)];

    /// <inheritdoc/>
    public async Task<OrderDto?> GetOrderAsync(int id)
    {
        var order = await orderRepository.GetByIdAsync(id);
        return order is null ? null : MapToDto(order);
    }

    /// <inheritdoc/>
    public async Task<OrderDto> AddOrderAsync(UpsertOrderDto order) =>
        MapToDto(await orderRepository.AddAsync(await MapToEntityAsync(order)));

    /// <inheritdoc/>
    public async Task<OrderDto?> UpdateOrderAsync(int id, UpsertOrderDto order)
    {
        if (await orderRepository.GetByIdAsync(id) is null)
        {
            return null;
        }

        var entity = await MapToEntityAsync(order);
        if (!await orderRepository.UpdateAsync(id, entity))
        {
            return null;
        }

        entity.OrderId = id;
        return MapToDto(entity);
    }

    /// <inheritdoc/>
    public Task<bool> DeleteOrderAsync(int id) => orderRepository.DeleteAsync(id);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrderDto>> GetInProcessingAsync() =>
        [.. (await orderRepository.GetInProcessingAsync()).Select(MapToDto)];

    /// <summary>
    /// Преобразует сущность в DTO
    /// </summary>
    private static OrderDto MapToDto(Order order) => new()
    {
        OrderId = order.OrderId,
        ClientId = order.Client.ClientId,
        ClientFullName = order.Client.FullName,
        ItemId = order.Item.ItemId,
        ItemName = order.Item.Name,
        ReceiptDate = order.ReceiptDate,
        CompletionTermDays = order.CompletionTermDays,
        Status = order.Status,
    };

    /// <summary>
    /// Преобразует DTO в сущность с проверкой данных,
    /// а также существования клиента и изделия
    /// </summary>
    private async Task<Order> MapToEntityAsync(UpsertOrderDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        return new Order
        {
            OrderId = 0,
            Client = await clientRepository.GetByIdAsync(
                    dto.ClientId ?? throw new ValidationException("Не указан клиент."))
                ?? throw new ValidationException("Клиент не найден."),
            Item = await itemRepository.GetByIdAsync(
                    dto.ItemId ?? throw new ValidationException("Не указано изделие."))
                ?? throw new ValidationException("Изделие не найдено."),
            ReceiptDate = dto.ReceiptDate
                ?? throw new ValidationException("Дата приёма заказа обязательна."),
            CompletionTermDays = dto.CompletionTermDays
                ?? throw new ValidationException("Срок выполнения обязателен."),
            Status = dto.Status
                ?? throw new ValidationException("Статус заказа обязателен."),
        };
    }
}
