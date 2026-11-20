using DryCleaning.Domain.Entities;

namespace DryCleaning.Domain.Interfaces;

/// <summary>
/// Репозиторий принятых изделий
/// </summary>
public interface IItemRepository : IRepository<Item>
{
    /// <summary>
    /// Проверяет, существует ли изделие, ссылающееся на категорию
    /// с указанным идентификатором. Используется при удалении категории:
    /// пока на неё ссылаются изделия, категорию удалить нельзя
    /// </summary>
    /// <param name="categoryId">Идентификатор категории</param>
    public Task<bool> HasCategoryAsync(int categoryId);
}
