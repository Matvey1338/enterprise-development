using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести сведения о клиенте, который потратил наибольшую сумму
/// за весь период работы химчистки"
///
/// Потраченная сумма - сумма стоимостей чистки по всем заказам клиента
/// за весь период; отменённые заказы не оплачиваются и не учитываются
/// </summary>
public sealed class TopSpenderClientTests : IClassFixture<DryCleaningFixture>
{
    private readonly DryCleaningFixture _fixture;

    public TopSpenderClientTests(DryCleaningFixture fixture)
    {
        _fixture = fixture;
    }

    private List<(Client Client, decimal TotalSpent)> QueryTotalSpending() =>
        _fixture.Clients
            .Select(c => (
                Client: c,
                TotalSpent: _fixture.Orders
                    .Where(o => o.Client.ClientId == c.ClientId && o.Status != OrderStatus.Cancelled)
                    .Sum(o => o.Item.Category.CleaningPrice)))
            .ToList();

    [Fact]
    public void ClientWithMaxTotalSpending_IsReturned()
    {
        // Act
        var topSpender = QueryTotalSpending()
            .OrderByDescending(x => x.TotalSpent)
            .First();

        // Assert: Петров - 1500 + 950 + 2500 + 1500 = 6450 руб. (заказы 4–6, 15)
        Assert.Equal("Петров Пётр Петрович", topSpender.Client.FullName);
        Assert.Equal(6450m, topSpender.TotalSpent);
    }

    [Fact]
    public void RunnerUpSpending_IsCalculatedCorrectly()
    {
        // Act
        var spending = QueryTotalSpending()
            .OrderByDescending(x => x.TotalSpent)
            .ToList();

        // Assert: Иванов на втором месте — 4750 руб. (заказы 0–3, 20)
        Assert.Equal("Иванов Иван Иванович", spending[1].Client.FullName);
        Assert.Equal(4750m, spending[1].TotalSpent);
    }

    [Fact]
    public void CancelledOrders_AreNotCountedInSpending()
    {
        // Act
        var spending = QueryTotalSpending();

        // Assert: единственный заказ Фёдорова (25) отменён — его трата равна 0
        var fyodorov = spending.Single(x => x.Client.FullName == "Фёдоров Сергей Павлович");
        Assert.Equal(0m, fyodorov.TotalSpent);
    }

    [Fact]
    public void OrdersOlderThanOneYear_AreCountedInAllTimeSpending()
    {
        // Act
        var spending = QueryTotalSpending();

        // Assert: в тратах за весь период учитываются и заказы старше года:
        // у Зайцевой заказ 23 (одеяло, 1000) + заказ 27 (шуба, 3200) = 4200 руб
        var zaytseva = spending.Single(x => x.Client.FullName == "Зайцева Наталья Викторовна");
        Assert.Equal(4200m, zaytseva.TotalSpent);
    }
}