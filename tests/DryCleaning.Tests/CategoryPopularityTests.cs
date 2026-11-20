using DryCleaning.Domain.Entities;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести топ-5 наиболее и наименее популярных категорий изделий
/// за последний год"
///
/// Популярность категории - число заказов на изделия этой категории за период
/// Категории без заказов считаются наименее популярными (0 заказов)
/// </summary>
public sealed class CategoryPopularityTests(DryCleaningFixture fixture) : IClassFixture<DryCleaningFixture>
{
    private IReadOnlyList<(Category Category, int OrdersCount)> QueryCategoryPopularity(DateOnly since)
    {
        var ordersCountByCategoryId = fixture.Orders
            .Where(o => o.ReceiptDate >= since)
            .GroupBy(o => o.Item.Category.CategoryId)
            .ToDictionary(g => g.Key, g => g.Count());

        return [.. fixture.Categories
            .Select(c => (Category: c, OrdersCount: ordersCountByCategoryId.GetValueOrDefault(c.CategoryId)))];
    }

    [Fact]
    public void Top5MostPopularCategories_ForLastYear_AreReturnedInExpectedOrder()
    {
        // Arrange: "последний год" - от опорной даты минус один год
        var since = fixture.ReferenceDate.AddYears(-1);
        string[] expectedNames = ["Верхняя одежда", "Костюмы", "Платья", "Рубашки", "Брюки"];
        int[] expectedOrdersCounts = [6, 5, 3, 3, 2];

        // Act
        var top5 = QueryCategoryPopularity(since)
            .OrderByDescending(x => x.OrdersCount)
            .ThenBy(x => x.Category.Name, StringComparer.Ordinal)
            .Take(5)
            .ToList();

        // Assert: "Платья" и "Рубашки" имеют по 3 заказа и упорядочены по названию
        Assert.Equal(expectedNames, top5.Select(x => x.Category.Name).ToArray());
        Assert.Equal(expectedOrdersCounts, top5.Select(x => x.OrdersCount).ToArray());
    }

    [Fact]
    public void Top5LeastPopularCategories_ForLastYear_AreReturnedInExpectedOrder()
    {
        // Arrange
        var since = fixture.ReferenceDate.AddYears(-1);
        string[] expectedNames = ["Одеяла", "Шторы", "Ковры", "Кожаные изделия", "Меховые изделия"];
        int[] expectedOrdersCounts = [0, 0, 1, 1, 1];

        // Act
        var bottom5 = QueryCategoryPopularity(since)
            .OrderBy(x => x.OrdersCount)
            .ThenBy(x => x.Category.Name, StringComparer.Ordinal)
            .Take(5)
            .ToList();

        // Assert
        Assert.Equal(expectedNames, bottom5.Select(x => x.Category.Name).ToArray());
        Assert.Equal(expectedOrdersCounts, bottom5.Select(x => x.OrdersCount).ToArray());
    }

    [Theory]
    [InlineData("Шторы", 0)]           // единственный заказ (22) старше года - не учитывается
    [InlineData("Одеяла", 0)]          // единственный заказ (23) старше года - не учитывается
    [InlineData("Меховые изделия", 1)] // свежий заказ 17 учитывается, а заказ 27 старше года - нет
    public void OrdersOlderThanOneYear_AreNotCounted(string categoryName, int expectedOrdersCount)
    {
        // Arrange
        var since = fixture.ReferenceDate.AddYears(-1);

        // Act
        var popularity = QueryCategoryPopularity(since);

        // Assert
        var category = popularity.Single(x => x.Category.Name == categoryName);
        Assert.Equal(expectedOrdersCount, category.OrdersCount);
    }
}