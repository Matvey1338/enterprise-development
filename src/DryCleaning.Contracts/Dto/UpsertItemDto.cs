using System.ComponentModel.DataAnnotations;

namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO принятого изделия: данные для создания и обновления
/// </summary>
public class UpsertItemDto
{
    /// <summary>
    /// Наименование изделия
    /// </summary>
    [Required(ErrorMessage = "Не указано наименование изделия")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Материал
    /// </summary>
    [Required(ErrorMessage = "Не указан материал")]
    public string Material { get; init; } = string.Empty;

    /// <summary>
    /// Идентификатор категории изделия
    /// </summary>
    [Required(ErrorMessage = "Не указана категория изделия")]
    [Range(0, int.MaxValue, ErrorMessage = "Некорректный идентификатор категории")]
    public int? CategoryId { get; init; }
}
