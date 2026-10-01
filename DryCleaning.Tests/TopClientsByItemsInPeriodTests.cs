using DryCleaning.Domain.Entities;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: «Вывести информацию о топ-5 клиентах, сдавших больше всего изделий
/// за заданный период». Каждый заказ соответствует ровно одному изделию.
/// </summary>
public sealed class TopClientsByItemsInPeriodTests : IClassFixture<DryCleaningFixture>
{
    private readonly DryCleaningFixture _fixture;

    public TopClientsByItemsInPeriodTests(DryCleaningFixture fixture)
    {
        _fixture = fixture;
    }

    // Заданный период: [сегодня - 100 дней; сегодня - 41 день], границы включаются
    private DateOnly PeriodFrom => _fixture.Today.AddDays(-100);
    private DateOnly PeriodTo => _fixture.Today.AddDays(-41);

    private List<(Client Client, int ItemsCount)> QueryTop5ClientsByItemsCount(DateOnly from, DateOnly to) =>
        _fixture.Orders
            .Where(o => o.ReceiptDate >= from && o.ReceiptDate <= to)
            .GroupBy(o => o.Client.ClientId)
            .Select(g => (Client: g.First().Client, ItemsCount: g.Count()))
            .OrderByDescending(x => x.ItemsCount)
            .ThenBy(x => x.Client.FullName, StringComparer.Ordinal)
            .Take(5)
            .ToList();

    [Fact]
    public void Top5Clients_ByItemsCountInPeriod_AreReturnedInExpectedOrder()
    {
        // Act
        var result = QueryTop5ClientsByItemsCount(PeriodFrom, PeriodTo);

        // Assert
        Assert.Equal(
            new[]
            {
                "Иванов Иван Иванович",          // 4 изделия
                "Петров Пётр Петрович",          // 3 изделия
                "Сидорова Анна Сергеевна",       // 3 изделия
                "Кузнецов Алексей Владимирович", // 2 изделия
                "Смирнова Елена Андреевна"       // 2 изделия
            },
            result.Select(x => x.Client.FullName).ToArray());
        Assert.Equal(new[] { 4, 3, 3, 2, 2 }, result.Select(x => x.ItemsCount).ToArray());
    }

    [Fact]
    public void ClientsWithSameItemsCount_AreOrderedByFullName()
    {
        // Act
        var result = QueryTop5ClientsByItemsCount(PeriodFrom, PeriodTo);

        // Assert: при равенстве числа изделий клиенты упорядочиваются по ФИО
        // (Петров/Сидорова - по 3, Кузнецов/Смирнова - по 2)
        Assert.Equal("Петров Пётр Петрович", result[1].Client.FullName);
        Assert.Equal("Сидорова Анна Сергеевна", result[2].Client.FullName);
        Assert.Equal("Кузнецов Алексей Владимирович", result[3].Client.FullName);
        Assert.Equal("Смирнова Елена Андреевна", result[4].Client.FullName);
    }

    [Fact]
    public void ClientWithSingleItemInPeriod_IsNotIncludedInTop5()
    {
        // Act
        var result = QueryTop5ClientsByItemsCount(PeriodFrom, PeriodTo);

        // Assert: Васильев сдал в периоде только 1 изделие (заказ 14)
        // и не попадает в топ-5, хотя всего у него два заказа
        Assert.DoesNotContain(result, x => x.Client.FullName == "Васильев Дмитрий Николаевич");
        Assert.Equal(5, result.Count);
    }

    [Fact]
    public void OrdersOutsidePeriod_AreNotCounted()
    {
        // Act
        var result = QueryTop5ClientsByItemsCount(PeriodFrom, PeriodTo);

        // Assert: заказ 15 (Петров, -1 день) и заказ 20 (Иванов, -3 дня)
        // находятся вне периода и не учитываются
        Assert.Equal(3, result.Single(x => x.Client.FullName == "Петров Пётр Петрович").ItemsCount);
        Assert.Equal(4, result.Single(x => x.Client.FullName == "Иванов Иван Иванович").ItemsCount);
    }
}