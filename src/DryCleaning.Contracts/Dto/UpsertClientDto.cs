using System.ComponentModel.DataAnnotations;

namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO клиента химчистки: данные для создания и обновления
/// </summary>
public class UpsertClientDto
{
    /// <summary>
    /// ФИО
    /// </summary>
    [Required(ErrorMessage = "Не указано ФИО")]
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Номер телефона
    /// </summary>
    [Required(ErrorMessage = "Не указан телефон")]
    [Phone(ErrorMessage = "Некорректный номер телефона")]
    public string Phone { get; init; } = string.Empty;
}
