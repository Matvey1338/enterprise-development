using System.ComponentModel.DataAnnotations;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO категории изделия: данные для создания и обновления
/// </summary>
public class UpsertCategoryDto
{
    /// <summary>
    /// Название категории
    /// </summary>
    [Required(ErrorMessage = "Не указано название категории")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Рекомендуемый вид чистки
    /// </summary>
    [Required(ErrorMessage = "Не указан рекомендуемый вид чистки")]
    [EnumDataType(typeof(CleaningType), ErrorMessage = "Некорректное значение вида чистки")]
    public CleaningType? RecommendedCleaningType { get; init; }

    /// <summary>
    /// Стоимость чистки изделия данной категории, руб
    /// </summary>
    [Required(ErrorMessage = "Не указана стоимость чистки")]
    [Range(typeof(decimal), "0.01", "10000000", ErrorMessage = "Стоимость чистки должна быть положительной")]
    public decimal? CleaningPrice { get; init; }
}
