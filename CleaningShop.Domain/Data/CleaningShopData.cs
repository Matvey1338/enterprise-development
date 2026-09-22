using CleaningShop.Domain.Entities;

namespace CleaningShop.Domain.Data;

/// <summary>Набор данных химчистки</summary>
public sealed record CleaningShopData(
    IReadOnlyList<Category> Categories,
    IReadOnlyList<Item> Items,
    IReadOnlyList<Client> Clients,
    IReadOnlyList<Order> Orders);