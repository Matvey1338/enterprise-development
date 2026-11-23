using DryCleaning.Contracts.Dto;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести топ-5 наиболее и наименее популярных категорий изделий
/// за последний год"
///
/// Популярность категории - число заказов на изделия этой категории за период
/// Категории без заказов считаются наименее популярными (0 заказов)
/// </summary>
public sealed class CategoryPopularityTests
{
    private readonly ServiceSet _services = ServiceSetFactory.Create();

    // "Последний год" сервис отсчитывает от часов, синхронизированных с данными
    private Task<IReadOnlyList<CategoryPopularityDto>> QueryMostPopular(int count = 5) =>
        _services.Categories.GetMostPopularCategoriesAsync(count);

    [Fact]
    public async Task Top5MostPopularCategories_ForLastYear_AreReturnedInExpectedOrder()
    {
        // Arrange: эталонные значения
        string[] expectedNames = ["Верхняя одежда", "Костюмы", "Платья", "Рубашки", "Брюки"];
        int[] expectedOrdersCounts = [6, 5, 3, 3, 2];

        // Act
        var top5 = await QueryMostPopular();

        // Assert: "Платья" и "Рубашки" имеют по 3 заказа и упорядочены по названию
        Assert.Equal(expectedNames, top5.Select(x => x.Name).ToArray());
        Assert.Equal(expectedOrdersCounts, top5.Select(x => x.OrdersCount).ToArray());
    }

    [Fact]
    public async Task Top5LeastPopularCategories_ForLastYear_AreReturnedInExpectedOrder()
    {
        // Arrange
        string[] expectedNames = ["Одеяла", "Шторы", "Ковры", "Кожаные изделия", "Меховые изделия"];
        int[] expectedOrdersCounts = [0, 0, 1, 1, 1];

        // Act
        var bottom5 = await _services.Categories.GetLeastPopularCategoriesAsync();

        // Assert
        Assert.Equal(expectedNames, bottom5.Select(x => x.Name).ToArray());
        Assert.Equal(expectedOrdersCounts, bottom5.Select(x => x.OrdersCount).ToArray());
    }

    [Theory]
    [InlineData("Шторы", 0)]           // единственный заказ (22) старше года - не учитывается
    [InlineData("Одеяла", 0)]          // единственный заказ (23) старше года - не учитывается
    [InlineData("Меховые изделия", 1)] // свежий заказ 17 учитывается, а заказ 27 старше года - нет
    public async Task OrdersOlderThanOneYear_AreNotCounted(string categoryName, int expectedOrdersCount)
    {
        // Arrange: полный рейтинг - count равно размеру справочника категорий (10),
        // чтобы в выборку попали все категории, включая непопулярные
        var popularity = await QueryMostPopular(10);

        // Assert
        var category = popularity.Single(x => x.Name == categoryName);
        Assert.Equal(expectedOrdersCount, category.OrdersCount);
    }
}
