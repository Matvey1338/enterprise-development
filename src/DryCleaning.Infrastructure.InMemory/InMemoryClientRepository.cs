using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Infrastructure.InMemory;

/// <summary>
/// Репозиторий клиентов химчистки с хранением в памяти
/// </summary>
/// <param name="seed">Начальные данные</param>
public class InMemoryClientRepository(IReadOnlyList<Client> seed)
    : InMemoryRepository<Client>(seed), IClientRepository
{
    /// <inheritdoc/>
    protected override int GetId(Client entity) => entity.ClientId;

    /// <inheritdoc/>
    protected override void SetId(Client entity, int id) => entity.ClientId = id;

    /// <inheritdoc/>
    protected override void CopyProperties(Client source, Client target)
    {
        target.FullName = source.FullName;
        target.Phone = source.Phone;
    }
}
