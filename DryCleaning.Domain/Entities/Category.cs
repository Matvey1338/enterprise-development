using DryCleaning.Domain.Enums;

namespace DryCleaning.Domain.Entities;

/// <summary>
/// Категория изделия (справочник)
/// </summary>
public sealed class Category
{
    /// <summary>Идентификатор</summary>
    public required int CategoryId { get; set; }

    /// <summary>Название категории</summary>
    public required string Name { get; set; }

    /// <summary>Рекомендуемый вид чистки</summary>
    public required CleaningType RecommendedCleaningType { get; set; }

    /// <summary>Стоимость чистки изделия данной категории, руб</summary>
    public required decimal CleaningPrice { get; set; }
}