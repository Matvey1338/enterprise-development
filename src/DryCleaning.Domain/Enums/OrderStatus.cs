namespace DryCleaning.Domain.Enums;

/// <summary>
/// Статус заказа
/// </summary>
public enum OrderStatus
{
    /// <summary>Принят в приёмный пункт</summary>
    Accepted = 1,

    /// <summary>Находится в обработке</summary>
    InProcessing = 2,

    /// <summary>Обработка завершена, готов к выдаче</summary>
    Completed = 3,

    /// <summary>Выдан клиенту</summary>
    Issued = 4,

    /// <summary>Отменён</summary>
    Cancelled = 5
}