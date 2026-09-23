using CleaningShop.Domain.Enums;

namespace CleaningShop.Domain.Reports;

/// <summary>Заказ в обработке (запрос №1)</summary>
public sealed record ProcessingOrderView(
    int OrderId,
    string ClientFullName,
    string ItemName,
    string CategoryName,
    DateOnly ReceivedDate,
    DateOnly DueDate,
    int DurationDays,
    OrderStatus Status);

/// <summary>Клиент и число сданных изделий за период (запрос №2)</summary>
public sealed record ClientItemsCountView(
    int ClientId,
    string ClientFullName,
    int ItemsCount);

/// <summary>Клиент и средняя длительность обработки его заказов (запрос №3)</summary>
public sealed record ClientProcessingDurationView(
    int ClientId,
    string ClientFullName,
    double AverageProcessingDays,
    int CompletedOrdersCount);

/// <summary>Категория и число заказов за период (запрос №4)</summary>
public sealed record CategoryPopularityView(
    int CategoryId,
    string CategoryName,
    int OrdersCount);

/// <summary>Клиент и потраченная сумма (запрос №5)</summary>
public sealed record ClientSpendingView(
    int ClientId,
    string ClientFullName,
    decimal TotalSpent);