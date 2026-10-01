using DryCleaning.Domain.Enums;

namespace DryCleaning.Domain.Entities;

/// <summary>
/// Заказ: содержит информацию о клиенте, изделии, дате приёма,
/// сроке выполнения в днях и статусе
/// </summary>
public sealed class Order
{
    /// <summary>Идентификатор</summary>
    public int OrderId { get; set; }

    /// <summary>Клиент, оформивший заказ</summary>
    public required Client Client { get; set; }

    /// <summary>Принятое изделие</summary>
    public required Item Item { get; set; }

    /// <summary>Дата приёма заказа</summary>
    public DateOnly ReceiptDate { get; set; }

    /// <summary>Срок выполнения, дней</summary>
    public int CompletionTermDays { get; set; }

    /// <summary>Статус заказа</summary>
    public OrderStatus Status { get; set; }
}