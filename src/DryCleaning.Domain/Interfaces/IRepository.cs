namespace DryCleaning.Domain.Interfaces;

/// <summary>
/// Базовый репозиторий: абстракция доступа к хранилищу сущностей
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Возвращает все сущности хранилища
    /// </summary>
    public Task<IReadOnlyList<TEntity>> GetAllAsync();

    /// <summary>
    /// Возвращает сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    public Task<TEntity?> GetByIdAsync(int id);

    /// <summary>
    /// Добавляет сущность, присваивая ей новый идентификатор
    /// </summary>
    /// <param name="entity">Сущность для добавления</param>
    /// <returns>Добавленная сущность с присвоенным идентификатором</returns>
    public Task<TEntity> AddAsync(TEntity entity);

    /// <summary>
    /// Обновляет свойства сущности с указанным идентификатором
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="entity">Новые данные сущности</param>
    /// <returns>true, если сущность найдена и обновлена./returns>
    public Task<bool> UpdateAsync(int id, TEntity entity);

    /// <summary>
    /// Удаляет сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <returns>true, если сущность найдена и удалена</returns>
    public Task<bool> DeleteAsync(int id);
}
