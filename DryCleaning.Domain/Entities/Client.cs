namespace DryCleaning.Domain.Entities;

/// <summary>
/// Клиент химчистки
/// </summary>
public sealed class Client
{
    /// <summary>Идентификатор</summary>
    public int ClientId { get; set; }

    /// <summary>ФИО</summary>
    public required string FullName { get; set; }

    /// <summary>Телефон</summary>
    public required string Phone { get; set; }
}