using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести информацию о заказах, находящихся в обработке,
/// упорядоченных по дате приема"
/// </summary>
public sealed class OrdersInProcessingTests : IClassFixture<DryCleaningFixture>
{
    private readonly DryCleaningFixture _fixture;

    public OrdersInProcessingTests(DryCleaningFixture fixture)
    {
        _fixture = fixture;
    }

    private List<Order> QueryOrdersInProcessing() =>
        _fixture.Orders
            .Where(o => o.Status == OrderStatus.InProcessing)
            .OrderBy(o => o.ReceiptDate)
            .ToList();

    [Fact]
    public void OrdersInProcessing_AreOrderedByReceiptDateAscending()
    {
        // Arrange: в фикстуре 4 заказа в обработке (Id 15–18) с датами приёма
        // -1, -12, -15 и -20 дней от текущей даты, а также заказы с другими статусами

        // Act: заказы в обработке, упорядоченные по дате приема (по возрастанию)
        var result = QueryOrdersInProcessing();

        // Assert: 4 заказа, от самой ранней даты приёма к самой поздней.
        Assert.Equal(new[] { 18, 17, 16, 15 }, result.Select(o => o.OrderId).ToArray());
        Assert.Equal(
            new[] { -20, -15, -12, -1 },
            result.Select(o => o.ReceiptDate.DayNumber - _fixture.Today.DayNumber).ToArray());
    }

    [Fact]
    public void OrdersInProcessing_ContainOnlyOrdersWithInProcessingStatus()
    {
        // Act
        var result = QueryOrdersInProcessing();

        // Assert: заказы с другими статусами не попадают в выборку,
        // в том числе свежие заказы 20 (принят) и 21 (выполнен)
        Assert.Equal(4, result.Count);
        Assert.All(result, o => Assert.Equal(OrderStatus.InProcessing, o.Status));
        Assert.DoesNotContain(result, o => o.OrderId is 20 or 21);
    }
}