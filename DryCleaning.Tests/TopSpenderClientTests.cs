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
public sealed class TopSpenderClientTests(DryCleaningFixture fixture) : IClassFixture<DryCleaningFixture>
{
    private IReadOnlyList<(Client Client, decimal TotalSpent)> QueryTotalSpending() =>
        [.. fixture.Clients
            .Select(c => (
                Client: c,
                TotalSpent: fixture.Orders
                    .Where(o => o.Client.ClientId == c.ClientId && o.Status != OrderStatus.Cancelled)
                    .Sum(o => o.Item.Category.CleaningPrice)))];

    [Fact]
    public void ClientWithMaxTotalSpending_IsReturned()
    {
        // Arrange: эталон - Петров: 1500 + 950 + 2500 + 1500 = 6450 руб (заказы 4-6, 15)
        const string expectedTopSpenderFullName = "Петров Пётр Петрович";
        const decimal expectedTotalSpent = 6450m;

        // Act
        var topSpender = QueryTotalSpending()
            .OrderByDescending(x => x.TotalSpent)
            .First();

        // Assert
        Assert.Equal(expectedTopSpenderFullName, topSpender.Client.FullName);
        Assert.Equal(expectedTotalSpent, topSpender.TotalSpent);
    }

    [Fact]
    public void RunnerUpSpending_IsCalculatedCorrectly()
    {
        // Arrange: эталон - Иванов на втором месте: 4750 руб (заказы 0-3, 20)
        const string expectedRunnerUpFullName = "Иванов Иван Иванович";
        const decimal expectedRunnerUpTotalSpent = 4750m;

        // Act
        var spending = QueryTotalSpending()
            .OrderByDescending(x => x.TotalSpent)
            .ToList();

        // Assert: вторая позиция рейтинга (индекс 1)
        Assert.Equal(expectedRunnerUpFullName, spending[1].Client.FullName);
        Assert.Equal(expectedRunnerUpTotalSpent, spending[1].TotalSpent);
    }

    [Theory]
    [InlineData("Фёдоров Сергей Павлович", 0.0)]       // единственный заказ (25) отменён и не оплачивается
    [InlineData("Зайцева Наталья Викторовна", 4200.0)] // заказы 23 и 27 старше года, но за весь период учитываются
    public void ClientTotalSpending_ForAllPeriod_IsExpected(string clientFullName, double expectedTotalSpent)
    {
        // Act
        var spending = QueryTotalSpending();

        // Assert
        var client = spending.Single(x => x.Client.FullName == clientFullName);
        Assert.Equal((decimal)expectedTotalSpent, client.TotalSpent);
    }
}