namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO строки результата запроса о популярности категорий:
/// категория изделия и число заказов по ней за период
/// Категории без заказов имеют нулевую популярность
/// </summary>
public sealed record CategoryPopularityDto
{
    /// <summary>
    /// Идентификатор категории
    /// </summary>
    public int CategoryId { get; init; }

    /// <summary>
    /// Название категории
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Число заказов на изделия данной категории за период
    /// </summary>
    public int OrdersCount { get; init; }
}
