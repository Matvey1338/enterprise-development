using CleaningShop.Domain.Enums;

namespace CleaningShop.Domain.Entities;

/// <summary>Заказ: клиент, изделие, дата приёма, срок выполнения в днях, статус</summary>
public sealed class Order
{
    public int Id { get; init; }

    public required Client Client { get; init; }

    public required Item Item { get; init; }

    /// <summary>Дата приёма заказа</summary>
    public DateOnly ReceivedDate { get; init; }

    /// <summary>Срок выполнения в днях</summary>
    public int DurationDays { get; init; }

    /// <summary>Статус заказа</summary>
    public OrderStatus Status { get; init; }

    /// <summary>
    /// Дата фактического завершения обработки (статусы «готов»/«выдан»)
    /// Дополнение к модели из задания: без неё нельзя посчитать,
    /// сколько заказ «обрабатывался» (запрос №3)
    /// </summary>
    public DateOnly? CompletedDate { get; init; }

    /// <summary>Ожидаемая дата готовности: дата приёма + срок выполнения</summary>
    public DateOnly DueDate => ReceivedDate.AddDays(DurationDays);
}