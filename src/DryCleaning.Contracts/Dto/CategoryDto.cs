using DryCleaning.Domain.Enums;

namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO категории изделия: данные в ответе сервера
/// </summary>
public class CategoryDto
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int CategoryId { get; init; }

    /// <summary>
    /// Название категории
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Рекомендуемый вид чистки
    /// </summary>
    public CleaningType RecommendedCleaningType { get; init; }

    /// <summary>
    /// Стоимость чистки изделия данной категории, руб
    /// </summary>
    public decimal CleaningPrice { get; init; }
}
