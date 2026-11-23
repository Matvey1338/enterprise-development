using DryCleaning.Contracts.Dto;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести сведения о клиенте, который потратил наибольшую сумму
/// за весь период работы химчистки"
///
/// Потраченная сумма - сумма стоимостей чистки по всем заказам клиента
/// за весь период; отменённые заказы не оплачиваются и не учитываются
/// </summary>
public sealed class TopSpenderClientTests
{
    private readonly ServiceSet _services = ServiceSetFactory.Create();

    [Fact]
    public async Task ClientWithMaxTotalSpending_IsReturned()
    {
        // Arrange: эталон - Петров: 1500 + 950 + 2500 + 1500 = 6450 руб (заказы 4-6, 15)
        const string expectedTopSpenderFullName = "Петров Пётр Петрович";
        const decimal expectedTotalSpent = 6450m;

        // Act
        ClientSpendingDto? topSpender = await _services.Clients.GetTopSpenderAsync();

        // Assert
        Assert.NotNull(topSpender);
        Assert.Equal(expectedTopSpenderFullName, topSpender?.FullName);
        Assert.Equal(expectedTotalSpent, topSpender?.TotalSpent);
    }

    [Fact]
    public async Task RunnerUpSpending_IsCalculatedCorrectly()
    {
        // Arrange: эталон - Иванов на втором месте: 4750 руб (заказы 0-3, 20)
        const string expectedRunnerUpFullName = "Иванов Иван Иванович";
        const decimal expectedRunnerUpTotalSpent = 4750m;

        // Act
        var spending = await _services.Clients.GetClientsSpendingAsync();

        // Assert: вторая позиция рейтинга (индекс 1)
        Assert.Equal(expectedRunnerUpFullName, spending[1].FullName);
        Assert.Equal(expectedRunnerUpTotalSpent, spending[1].TotalSpent);
    }

    [Theory]
    [InlineData("Фёдоров Сергей Павлович", 0.0)]       // единственный заказ (25) отменён и не оплачивается
    [InlineData("Зайцева Наталья Викторовна", 4200.0)] // заказы 23 и 27 старше года, но за весь период учитываются
    public async Task ClientTotalSpending_ForAllPeriod_IsExpected(string clientFullName, double expectedTotalSpent)
    {
        // Act
        var spending = await _services.Clients.GetClientsSpendingAsync();

        // Assert
        var client = spending.Single(x => x.FullName == clientFullName);
        Assert.Equal((decimal)expectedTotalSpent, client.TotalSpent);
    }
}
