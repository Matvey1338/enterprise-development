using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Seeds;

namespace DryCleaning.Tests;

/// <summary>
/// Фикстура, содержащая тестовые данные химчистки.
/// </summary>
public class DryCleaningFixture
{
    /// <summary>
    /// Момент времени, относительно которого построены тестовые данные
    /// </summary>
    public DateTime Now { get; }

    /// <summary>
    /// Дата, относительно которой построены тестовые данные
    /// </summary>
    public DateOnly ReferenceDate { get; }

    /// <summary>
    /// Набор данных химчистки, созданный генератором
    /// </summary>
    public DryCleaningData Data { get; }

    /// <summary>
    /// Справочник категорий изделий
    /// </summary>
    public IReadOnlyList<Category> Categories => Data.Categories;

    /// <summary>
    /// Клиенты химчистки
    /// </summary>
    public IReadOnlyList<Client> Clients => Data.Clients;

    /// <summary>
    /// Принятые изделия (по одному на каждый заказ, ItemId = OrderId)
    /// </summary>
    public IReadOnlyList<Item> Items => Data.Items;

    /// <summary>
    /// Заказы химчистки
    /// </summary>
    public IReadOnlyList<Order> Orders => Data.Orders;

    /// <summary>
    /// Инициализирует тестовые данные
    /// </summary>
    public DryCleaningFixture()
    {
        Now = DateTime.Now;
        ReferenceDate = DateOnly.FromDateTime(Now);
        Data = DryCleaningDataSeeder.Create(Now);
    }
}
