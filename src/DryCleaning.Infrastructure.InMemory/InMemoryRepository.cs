using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Infrastructure.InMemory;

/// <summary>
/// Репозиторий с хранением в памяти и последовательным выделением идентификаторов.
/// </summary>
/// <typeparam name="TEntity">Тип сущности.</typeparam>
public abstract class InMemoryRepository<TEntity> : IRepository<TEntity>
    where TEntity : class
{
    private readonly List<TEntity> _items;
    private readonly Lock _lock = new();
    private int _nextId;

    /// <summary>
    /// Создаёт репозиторий с начальными данными
    /// </summary>
    /// <param name="seed">Начальные данные</param>
    protected InMemoryRepository(IReadOnlyList<TEntity> seed)
    {
        _items = [.. seed];
        _nextId = seed.Count == 0 ? 0 : seed.Max(item => GetId(item)) + 1;
    }

    /// <inheritdoc/>
    public Task<IReadOnlyList<TEntity>> GetAllAsync() =>
        ReadAsync<IReadOnlyList<TEntity>>(items => [.. items]);

    /// <inheritdoc/>
    public Task<TEntity?> GetByIdAsync(int id) =>
        ReadAsync(items => items.FirstOrDefault(item => GetId(item) == id));

    /// <inheritdoc/>
    public Task<TEntity> AddAsync(TEntity entity)
    {
        lock (_lock)
        {
            if (_items.Any(item => ReferenceEquals(item, entity)))
            {
                throw new InvalidOperationException("Объект уже добавлен в репозиторий.");
            }

            SetId(entity, _nextId++);
            _items.Add(entity);
            return Task.FromResult(entity);
        }
    }

    /// <inheritdoc/>
    public Task<bool> UpdateAsync(int id, TEntity entity)
    {
        lock (_lock)
        {
            var existing = _items.FirstOrDefault(item => GetId(item) == id);
            if (existing is null)
            {
                return Task.FromResult(false);
            }

            CopyProperties(entity, existing);
            return Task.FromResult(true);
        }
    }

    /// <inheritdoc/>
    public Task<bool> DeleteAsync(int id)
    {
        lock (_lock)
        {
            return Task.FromResult(_items.RemoveAll(item => GetId(item) == id) > 0);
        }
    }

    /// <summary>
    /// Выполняет запрос под блокировкой, пока коллекция не меняется
    /// </summary>
    protected Task<TResult> ReadAsync<TResult>(Func<IReadOnlyList<TEntity>, TResult> query)
    {
        lock (_lock)
        {
            return Task.FromResult(query(_items));
        }
    }

    /// <summary>
    /// Возвращает идентификатор сущности
    /// </summary>
    protected abstract int GetId(TEntity entity);

    /// <summary>
    /// Присваивает сущности новый идентификатор при добавлении в хранилище
    /// </summary>
    protected abstract void SetId(TEntity entity, int id);

    /// <summary>
    /// Копирует изменяемые свойства, сохраняя идентификатор и ссылки на существующий объект
    /// </summary>
    protected abstract void CopyProperties(TEntity source, TEntity target);
}
