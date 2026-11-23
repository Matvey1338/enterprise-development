using DryCleaning.Contracts.Dto;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести информацию о топ-5 клиентах, сдавших больше всего изделий
/// за заданный период". Каждый заказ соответствует ровно одному изделию
/// </summary>
public sealed class TopClientsByItemsInPeriodTests
{
    private readonly ServiceSet _services = ServiceSetFactory.Create();

    // Заданный период: [опорная дата - 100 дней; опорная дата - 41 день], границы включаются
    private DateOnly PeriodFrom => _services.ReferenceDate.AddDays(-100);
    private DateOnly PeriodTo => _services.ReferenceDate.AddDays(-41);

    private Task<IReadOnlyList<ClientItemsCountDto>> QueryTop5ClientsByItemsCount() =>
        _services.Clients.GetTopClientsByItemsCountAsync(PeriodFrom, PeriodTo);

    [Fact]
    public async Task Top5Clients_ByItemsCountInPeriod_AreReturnedInExpectedOrder()
    {
        // Arrange: эталонные значения
        string[] expectedFullNames =
        [
            "Иванов Иван Иванович",          // 4 изделия
            "Петров Пётр Петрович",          // 3 изделия
            "Сидорова Анна Сергеевна",       // 3 изделия
            "Кузнецов Алексей Владимирович", // 2 изделия
            "Смирнова Елена Андреевна"       // 2 изделия
        ];
        int[] expectedItemsCounts = [4, 3, 3, 2, 2];

        // Act
        var result = await QueryTop5ClientsByItemsCount();

        // Assert
        Assert.Equal(expectedFullNames, result.Select(x => x.FullName).ToArray());
        Assert.Equal(expectedItemsCounts, result.Select(x => x.ItemsCount).ToArray());
    }

    [Fact]
    public async Task ClientsWithSameItemsCount_AreOrderedByFullName()
    {
        // Arrange: эталон - при равенстве числа изделий клиенты упорядочиваются по ФИО:
        // Петров/Сидорова (по 3 изделия) - позиции 2-3, Кузнецов/Смирнова (по 2) - позиции 4-5
        string[] expectedFullNamesAfterLeader =
        [
            "Петров Пётр Петрович",
            "Сидорова Анна Сергеевна",
            "Кузнецов Алексей Владимирович",
            "Смирнова Елена Андреевна"
        ];

        // Act
        var result = await QueryTop5ClientsByItemsCount();

        // Assert
        Assert.Equal(expectedFullNamesAfterLeader, result.Skip(1).Select(x => x.FullName).ToArray());
    }

    [Fact]
    public async Task ClientWithSingleItemInPeriod_IsNotIncludedInTop5()
    {
        // Arrange: эталон - Васильев сдал в периоде только 1 изделие (заказ 14)
        // и не попадает в топ-5, хотя всего у него два заказа
        const string clientOutsideTop5 = "Васильев Дмитрий Николаевич";
        const int expectedTopSize = 5;

        // Act
        var result = await QueryTop5ClientsByItemsCount();

        // Assert
        Assert.DoesNotContain(result, x => x.FullName == clientOutsideTop5);
        Assert.Equal(expectedTopSize, result.Count);
    }

    [Theory]
    [InlineData("Петров Пётр Петрович", 3)] // заказ 15 (-1 день) вне периода - не учитывается
    [InlineData("Иванов Иван Иванович", 4)] // заказ 20 (-3 дня) вне периода - не учитывается
    public async Task OrdersOutsidePeriod_AreNotCounted(string clientFullName, int expectedItemsCount)
    {
        // Act
        var result = await QueryTop5ClientsByItemsCount();

        // Assert
        var client = result.Single(x => x.FullName == clientFullName);
        Assert.Equal(expectedItemsCount, client.ItemsCount);
    }
}
