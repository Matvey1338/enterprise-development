using DryCleaning.Contracts.Dto;

namespace DryCleaning.Contracts.Interfaces;

/// <summary>
/// Сервис заказов химчистки
/// </summary>
public interface IOrderService
{
    /// <summary>
    /// Возвращает список всех заказов
    /// </summary>
    public Task<IReadOnlyList<OrderDto>> GetOrdersAsync();

    /// <summary>
    /// Возвращает заказ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    public Task<OrderDto?> GetOrderAsync(int id);

    /// <summary>
    /// Добавляет заказ и возвращает созданную сущность с присвоенным идентификатором
    /// </summary>
    /// <param name="order">Данные заказа</param>
    public Task<OrderDto> AddOrderAsync(UpsertOrderDto order);

    /// <summary>
    /// Заменяет заказ с указанным идентификатором
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <param name="order">Новые данные заказа</param>
    /// <returns>Обновлённый заказ или null, если он не найден</returns>
    public Task<OrderDto?> UpdateOrderAsync(int id, UpsertOrderDto order);

    /// <summary>
    /// Удаляет заказ по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор заказа</param>
    /// <returns>true, если заказ найден и удалён</returns>
    public Task<bool> DeleteOrderAsync(int id);

    /// <summary>
    /// Возвращает заказы, находящиеся в обработке,
    /// упорядоченные по дате приёма
    /// </summary>
    public Task<IReadOnlyList<OrderDto>> GetInProcessingAsync();
}
