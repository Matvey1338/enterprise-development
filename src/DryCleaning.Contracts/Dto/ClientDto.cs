namespace DryCleaning.Contracts.Dto;

/// <summary>
/// DTO клиента химчистки: данные в ответе сервера
/// </summary>
public class ClientDto
{
    /// <summary>
    /// Идентификатор
    /// </summary>
    public int ClientId { get; init; }

    /// <summary>
    /// ФИО
    /// </summary>
    public string FullName { get; init; } = string.Empty;

    /// <summary>
    /// Номер телефона
    /// </summary>
    public string Phone { get; init; } = string.Empty;
}
