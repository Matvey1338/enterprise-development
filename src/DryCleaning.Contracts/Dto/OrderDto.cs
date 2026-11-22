using DryCleaning.Domain.Enums;

namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO заказа: данные в ответе сервера
/// </summary>
public class OrderDto
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int OrderId { get; init; }

    /// <summary>
    /// Идентификатор клиента, оформившего заказ
    /// </summary>
    public int ClientId { get; init; }

    /// <summary>
    /// ФИО клиента, оформившего заказ
    /// </summary>
    public string ClientFullName { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор принятого изделия
    /// </summary>
    public int ItemId { get; init; }

    /// <summary>
    /// Наименование принятого изделия
    /// </summary>
    public string ItemName { get; init; } = string.Empty;

    /// <summary>
    /// Дата приёма заказа
    /// </summary>
    public DateOnly ReceiptDate { get; init; }

    /// <summary>
    /// Срок выполнения, дней
    /// </summary>
    public int CompletionTermDays { get; init; }

    /// <summary>
    /// Статус заказа
    /// </summary>
    public OrderStatus Status { get; init; }
}
