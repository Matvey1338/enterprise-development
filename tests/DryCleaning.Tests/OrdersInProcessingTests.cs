using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Tests;

/// <summary>
/// Запрос: "Вывести информацию о заказах, находящихся в обработке,
/// упорядоченных по дате приема"
/// </summary>
public sealed class OrdersInProcessingTests(DryCleaningFixture fixture) : IClassFixture<DryCleaningFixture>
{
    private IReadOnlyList<Order> QueryOrdersInProcessing() =>
        [.. fixture.Orders
            .Where(o => o.Status == OrderStatus.InProcessing)
            .OrderBy(o => o.ReceiptDate)];

    [Fact]
    public void OrdersInProcessing_AreOrderedByReceiptDateAscending()
    {
        // Arrange: эталон - 4 заказа в обработке (OrderId 15-18) с датами приёма
        // -1, -12, -15 и -20 дней от опорной даты, от ранней даты к поздней
        int[] expectedOrderIds = [18, 17, 16, 15];
        int[] expectedReceiptDayOffsets = [-20, -15, -12, -1];

        // Act
        var result = QueryOrdersInProcessing();

        // Assert
        Assert.Equal(expectedOrderIds, result.Select(o => o.OrderId).ToArray());
        Assert.Equal(
            expectedReceiptDayOffsets,
            result.Select(o => o.ReceiptDate.DayNumber - fixture.ReferenceDate.DayNumber).ToArray());
    }

    [Fact]
    public void OrdersInProcessing_ContainOnlyOrdersWithInProcessingStatus()
    {
        // Arrange: эталон - 4 заказа в обработке; свежие заказы 20 (принят)
        // и 21 (выполнен) в выборку не попадают
        const int expectedOrdersCount = 4;

        // Act
        var result = QueryOrdersInProcessing();

        // Assert
        Assert.Equal(expectedOrdersCount, result.Count);
        Assert.All(result, o => Assert.Equal(OrderStatus.InProcessing, o.Status));
        Assert.DoesNotContain(result, o => o.OrderId is 20 or 21);
    }
}