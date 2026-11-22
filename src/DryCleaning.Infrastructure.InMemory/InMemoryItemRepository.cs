using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Infrastructure.InMemory;

/// <summary>
/// Репозиторий принятых изделий с хранением в памяти
/// </summary>
/// <param name="seed">Начальные данные</param>
public class InMemoryItemRepository(IReadOnlyList<Item> seed)
    : InMemoryRepository<Item>(seed), IItemRepository
{
    /// <inheritdoc/>
    public Task<bool> HasCategoryAsync(int categoryId) =>
        ReadAsync(items => items.Any(item => item.Category.CategoryId == categoryId));

    /// <inheritdoc/>
    protected override int GetId(Item entity) => entity.ItemId;

    /// <inheritdoc/>
    protected override void SetId(Item entity, int id) => entity.ItemId = id;

    /// <inheritdoc/>
    protected override void CopyProperties(Item source, Item target)
    {
        target.Name = source.Name;
        target.Material = source.Material;
        target.Category = source.Category;
    }
}
