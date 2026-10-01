using DryCleaning.Domain.Entities;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести топ-5 наиболее и наименее популярных категорий изделий
/// за последний год"
///
/// Популярность категории - число заказов на изделия этой категории за период
/// Категории без заказов считаются наименее популярными (0 заказов)
/// </summary>
public sealed class CategoryPopularityTests : IClassFixture<DryCleaningFixture>
{
    private readonly DryCleaningFixture _fixture;

    public CategoryPopularityTests(DryCleaningFixture fixture)
    {
        _fixture = fixture;
    }

    private List<(Category Category, int OrdersCount)> QueryCategoryPopularity(DateOnly since)
    {
        var ordersCountByCategoryId = _fixture.Orders
            .Where(o => o.ReceiptDate >= since)
            .GroupBy(o => o.Item.Category.CategoryId)
            .ToDictionary(g => g.Key, g => g.Count());

        return _fixture.Categories
            .Select(c => (Category: c, OrdersCount: ordersCountByCategoryId.GetValueOrDefault(c.CategoryId)))
            .ToList();
    }

    [Fact]
    public void Top5MostPopularCategories_ForLastYear_AreReturnedInExpectedOrder()
    {
        // Arrange: "последний год" - от текущей даты минус один год
        var since = _fixture.Today.AddYears(-1);

        // Act
        var top5 = QueryCategoryPopularity(since)
            .OrderByDescending(x => x.OrdersCount)
            .ThenBy(x => x.Category.Name, StringComparer.Ordinal)
            .Take(5)
            .ToList();

        // Assert: "Платья" и "Рубашки" имеют по 3 заказа и упорядочены по названию
        Assert.Equal(
            new[] { "Верхняя одежда", "Костюмы", "Платья", "Рубашки", "Брюки" },
            top5.Select(x => x.Category.Name).ToArray());
        Assert.Equal(new[] { 6, 5, 3, 3, 2 }, top5.Select(x => x.OrdersCount).ToArray());
    }

    [Fact]
    public void Top5LeastPopularCategories_ForLastYear_AreReturnedInExpectedOrder()
    {
        // Arrange
        var since = _fixture.Today.AddYears(-1);

        // Act
        var bottom5 = QueryCategoryPopularity(since)
            .OrderBy(x => x.OrdersCount)
            .ThenBy(x => x.Category.Name, StringComparer.Ordinal)
            .Take(5)
            .ToList();

        // Assert: "Одеяла" и "Шторы" не имели заказов за последний год
        // (их заказы приняты больше года назад)
        Assert.Equal(
            new[] { "Одеяла", "Шторы", "Ковры", "Кожаные изделия", "Меховые изделия" },
            bottom5.Select(x => x.Category.Name).ToArray());
        Assert.Equal(new[] { 0, 0, 1, 1, 1 }, bottom5.Select(x => x.OrdersCount).ToArray());
    }

    [Fact]
    public void OrdersOlderThanOneYear_AreNotCounted()
    {
        // Arrange
        var since = _fixture.Today.AddYears(-1);

        // Act
        var popularity = QueryCategoryPopularity(since);

        // Assert: заказы на шторы (22) и одеяла (23) приняты больше года назад,
        // поэтому за последний год у этих категорий 0 заказов.
        Assert.Equal(0, popularity.Single(x => x.Category.Name == "Шторы").OrdersCount);
        Assert.Equal(0, popularity.Single(x => x.Category.Name == "Одеяла").OrdersCount);
    }
}