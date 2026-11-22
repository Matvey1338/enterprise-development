namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO принятого изделия: данные в ответе сервера
/// </summary>
public class ItemDto
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int ItemId { get; init; }

    /// <summary>
    /// Наименование изделия
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Материал
    /// </summary>
    public string Material { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор категории изделия
    /// </summary>
    public int CategoryId { get; init; }

    /// <summary>
    /// Название категории изделия
    /// </summary>
    public string CategoryName { get; init; } = string.Empty;
}
