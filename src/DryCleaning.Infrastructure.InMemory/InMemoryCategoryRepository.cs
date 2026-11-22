using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Infrastructure.InMemory;

/// <summary>
/// Репозиторий категорий изделий с хранением в памяти
/// </summary>
/// <param name="seed">Начальные данные</param>
public class InMemoryCategoryRepository(IReadOnlyList<Category> seed)
    : InMemoryRepository<Category>(seed), ICategoryRepository
{
    /// <inheritdoc/>
    protected override int GetId(Category entity) => entity.CategoryId;

    /// <inheritdoc/>
    protected override void SetId(Category entity, int id) => entity.CategoryId = id;

    /// <inheritdoc/>
    protected override void CopyProperties(Category source, Category target)
    {
        target.Name = source.Name;
        target.RecommendedCleaningType = source.RecommendedCleaningType;
        target.CleaningPrice = source.CleaningPrice;
    }
}
