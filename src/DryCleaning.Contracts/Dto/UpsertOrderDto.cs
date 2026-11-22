using System.ComponentModel.DataAnnotations;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO заказа: данные для создания и обновления
/// </summary>
public class UpsertOrderDto
{
    /// <summary>
    /// Идентификатор клиента, оформившего заказ
    /// </summary>
    [Required(ErrorMessage = "Не указан клиент")]
    [Range(0, int.MaxValue, ErrorMessage = "Некорректный идентификатор клиента")]
    public int? ClientId { get; init; }

    /// <summary>
    /// Идентификатор принятого изделия
    /// </summary>
    [Required(ErrorMessage = "Не указано изделие")]
    [Range(0, int.MaxValue, ErrorMessage = "Некорректный идентификатор изделия")]
    public int? ItemId { get; init; }

    /// <summary>
    /// Дата приёма заказа
    /// </summary>
    [Required(ErrorMessage = "Не указана дата приёма заказа")]
    public DateOnly? ReceiptDate { get; init; }

    /// <summary>
    /// Срок выполнения, дней
    /// </summary>
    [Required(ErrorMessage = "Не указан срок выполнения")]
    [Range(1, 365, ErrorMessage = "Срок выполнения должен быть от 1 до 365 дней")]
    public int? CompletionTermDays { get; init; }

    /// <summary>
    /// Статус заказа
    /// </summary>
    [Required(ErrorMessage = "Не указан статус заказа")]
    [EnumDataType(typeof(OrderStatus), ErrorMessage = "Некорректное значение статуса заказа")]
    public OrderStatus? Status { get; init; }
}
