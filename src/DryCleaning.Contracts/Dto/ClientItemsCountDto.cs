namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO строки результата запроса "топ клиентов по числу сданных изделий":
/// клиент и число его изделий за период
/// </summary>
public sealed record ClientItemsCountDto
{
    /// <summary>
    /// Идентификатор клиента
    /// </summary>
    public int ClientId { get; init; }

    /// <summary>
    /// ФИО клиента
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Число изделий, сданных клиентом за период
    /// </summary>
    public int ItemsCount { get; init; }
}
