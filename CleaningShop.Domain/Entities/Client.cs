namespace CleaningShop.Domain.Entities;

/// <summary>Клиент химчистки: ФИО, телефон</summary>
public sealed class Client
{
    public int Id { get; init; }

    public required string FullName { get; init; }

    public required string Phone { get; init; }
}