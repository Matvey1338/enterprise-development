using System.ComponentModel.DataAnnotations;
using DryCleaning.Domain.Enums;

namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO категории изделия: данные для создания и обновления
/// </summary>
public class UpsertCategoryDto : IValidatableObject
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
    public decimal? CleaningPrice { get; init; }

    /// <inheritdoc/>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CleaningPrice is <= 0)
        {
            yield return new ValidationResult(
                "Стоимость чистки должна быть положительной.",
                [nameof(CleaningPrice)]);
        }
    }
}
