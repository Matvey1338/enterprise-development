using DryCleaning.Domain.Entities;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести информацию о клиентах, чьи заказы обрабатывались дольше всего,
/// упорядочить по ФИО"
///
/// Длительность обработки заказа определяется его сроком выполнения
/// (CompletionTermDays)
/// </summary>
public sealed class ClientsWithLongestProcessingTests : IClassFixture<DryCleaningFixture>
{
    private readonly DryCleaningFixture _fixture;

    public ClientsWithLongestProcessingTests(DryCleaningFixture fixture)
    {
        _fixture = fixture;
    }

    private List<Client> QueryClientsWithLongestProcessing()
    {
        var maxTermDays = _fixture.Orders.Max(o => o.CompletionTermDays);

        return _fixture.Orders
            .Where(o => o.CompletionTermDays == maxTermDays)
            .Select(o => o.Client)
            .DistinctBy(c => c.ClientId)
            .OrderBy(c => c.FullName, StringComparer.Ordinal)
            .ToList();
    }

    [Fact]
    public void ClientsOfOrdersWithMaxTerm_AreOrderedByFullName()
    {
        // Act
        var result = QueryClientsWithLongestProcessing();

        // Assert: максимальный срок - 14 дней (заказы 17, 18 и 22),
        // клиенты упорядочены по ФИО
        Assert.Equal(
            new[] { "Морозова Ольга Ивановна", "Соловьёв Игорь Олегович" },
            result.Select(c => c.FullName).ToArray());
    }

    [Fact]
    public void ClientWithSeveralLongestOrders_AppearsOnlyOnce()
    {
        // Act
        var result = QueryClientsWithLongestProcessing();

        // Assert: у Морозовой два заказа со сроком 14 дней (17 и 22),
        // но в результат она входит один раз
        Assert.Equal(2, result.Count);
        Assert.Single(result, c => c.FullName == "Морозова Ольга Ивановна");
    }

    [Fact]
    public void ClientsWithoutMaxTermOrders_AreExcluded()
    {
        // Act
        var result = QueryClientsWithLongestProcessing();

        // Assert: максимальный срок заказов Петрова - 10 дней (заказ 6),
        // поэтому его в результате быть не должно
        Assert.DoesNotContain(result, c => c.FullName == "Петров Пётр Петрович");
    }
}