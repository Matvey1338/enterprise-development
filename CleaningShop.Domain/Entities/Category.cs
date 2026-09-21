using CleaningShop.Domain.Enums;

namespace CleaningShop.Domain.Entities;

/// <summary>Категория изделия (справочник): название, вид чистки, стоимость</summary>
public sealed class Category
{
    public int Id { get; init; }

    /// <summary>Название категории (например, «Пальто»).</summary>
    public required string Name { get; init; }

    /// <summary>Рекомендуемый вид чистки для изделий категории</summary>
    public CleaningType RecommendedCleaning { get; init; }

    /// <summary>Стоимость чистки одного изделия категории</summary>
    public decimal CleaningPrice { get; init; }
}