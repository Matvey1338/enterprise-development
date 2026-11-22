namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO результата запроса о клиенте с наибольшей суммой трат:
/// клиент и потраченная им сумма за весь период работы химчистки
/// </summary>
public sealed record ClientSpendingDto
{
    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public int ClientId { get; init; }

    /// <summary>
    /// ФИО клиента
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Сумма потраченных клиентом средств за весь период, руб.
    /// Отменённые заказы не оплачиваются и не учитываются
    /// </summary>
    public decimal TotalSpent { get; init; }
}
