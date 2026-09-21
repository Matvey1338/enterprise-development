namespace CleaningShop.Domain.Entities;

/// <summary>Изделие, принимаемое в химчистку: наименование, материал, категория</summary>
public sealed class Item
{
    public int Id { get; init; }

    /// <summary>Наименование изделия</summary>
    public required string Name { get; init; }

    /// <summary>Материал изделия</summary>
    public required string Material { get; init; }

    /// <summary>Категория изделия</summary>
    public required Category Category { get; init; }
}