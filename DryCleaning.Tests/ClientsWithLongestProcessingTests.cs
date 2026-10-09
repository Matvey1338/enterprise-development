using DryCleaning.Domain.Entities;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести информацию о клиентах, чьи заказы обрабатывались дольше всего,
/// упорядочить по ФИО"
///
/// Длительность обработки заказа определяется его сроком выполнения
/// (CompletionTermDays)
/// </summary>
public sealed class ClientsWithLongestProcessingTests(DryCleaningFixture fixture) : IClassFixture<DryCleaningFixture>
{
    private IReadOnlyList<Client> QueryClientsWithLongestProcessing()
    {
        var maxTermDays = fixture.Orders.Max(o => o.CompletionTermDays);

        return [.. fixture.Orders
            .Where(o => o.CompletionTermDays == maxTermDays)
            .Select(o => o.Client)
            .DistinctBy(c => c.ClientId)
            .OrderBy(c => c.FullName, StringComparer.Ordinal)];
    }

    [Fact]
    public void ClientsOfOrdersWithMaxTerm_AreOrderedByFullName()
    {
        // Arrange: эталон - максимальный срок 14 дней (заказы 17, 18 и 22),
        // клиенты упорядочены по ФИО
        string[] expectedFullNames = ["Морозова Ольга Ивановна", "Соловьёв Игорь Олегович"];

        // Act
        var result = QueryClientsWithLongestProcessing();

        // Assert
        Assert.Equal(expectedFullNames, result.Select(c => c.FullName).ToArray());
    }

    [Fact]
    public void ClientWithSeveralLongestOrders_AppearsOnlyOnce()
    {
        // Arrange: эталон - у Морозовой два заказа со сроком 14 дней (17 и 22),
        // но в результат она входит один раз
        const int expectedClientsCount = 2;
        const string clientWithSeveralLongestOrders = "Морозова Ольга Ивановна";

        // Act
        var result = QueryClientsWithLongestProcessing();

        // Assert
        Assert.Equal(expectedClientsCount, result.Count);
        Assert.Single(result, c => c.FullName == clientWithSeveralLongestOrders);
    }

    [Fact]
    public void ClientsWithoutMaxTermOrders_AreExcluded()
    {
        // Arrange: эталон - максимальный срок заказов Петрова - 10 дней (заказ 6),
        // поэтому его в результате быть не должно
        const string clientWithoutMaxTermOrders = "Петров Пётр Петрович";

        // Act
        var result = QueryClientsWithLongestProcessing();

        // Assert
        Assert.DoesNotContain(result, c => c.FullName == clientWithoutMaxTermOrders);
    }
}