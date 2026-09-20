namespace CleaningShop.Domain.Enums;

/// <summary>Статус заказа</summary>
public enum OrderStatus
{
    /// <summary>Принят</summary>
    Received,

    /// <summary>В обработке</summary>
    Processing,

    /// <summary>Готов</summary>
    Completed,

    /// <summary>Выдан</summary>
    Issued,

    /// <summary>Отменён</summary>
    Cancelled,
}