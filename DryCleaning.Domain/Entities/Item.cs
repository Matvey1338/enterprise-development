namespace DryCleaning.Domain.Entities;

/// <summary>
/// Принятое изделие: характеризуется наименованием, категорией и материалом.
/// Каждое изделие относится ровно к одной категории и входит ровно в один заказ
/// </summary>
public sealed class Item
{
    /// <summary>Идентификатор</summary>
    public int ItemId { get; set; }

    /// <summary>Наименование изделия</summary>
    public required string Name { get; set; }

    /// <summary>Материал</summary>
    public required string Material { get; set; }

    /// <summary>Категория изделия (справочник)</summary>
    public required Category Category { get; set; }
}