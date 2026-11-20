using DryCleaning.Domain.Entities;

namespace DryCleaning.Domain.Seeds;

/// <summary>
/// Набор данных химчистки для инициализации хранилища:
/// справочник категорий, клиенты, принятые изделия и заказы.
/// Один и тот же набор используется при старте сервера и в тестах.
/// </summary>
public sealed class DryCleaningData
{
    /// <summary>Справочник категорий изделий</summary>
    public required IReadOnlyList<Category> Categories { get; init; }

    /// <summary>Клиенты химчистки</summary>
    public required IReadOnlyList<Client> Clients { get; init; }

    /// <summary>Принятые изделия</summary>
    public required IReadOnlyList<Item> Items { get; init; }

    /// <summary>Заказы химчистки</summary>
    public required IReadOnlyList<Order> Orders { get; init; }
}
