using DryCleaning.Contracts.Dto;

namespace DryCleaning.Contracts.Interfaces;

/// <summary>
/// Сервис принятых изделий химчистки
/// </summary>
public interface IItemService
{
    /// <summary>
    /// Возвращает список всех принятых изделий
    /// </summary>
    public Task<IReadOnlyList<ItemDto>> GetItemsAsync();

    /// <summary>
    /// Возвращает изделие по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор изделия</param>
    public Task<ItemDto?> GetItemAsync(int id);

    /// <summary>
    /// Добавляет изделие и возвращает созданную сущность с присвоенным идентификатором
    /// </summary>
    /// <param name="item">Данные изделия</param>
    public Task<ItemDto> AddItemAsync(UpsertItemDto item);

    /// <summary>
    /// Заменяет изделие с указанным идентификатором
    /// </summary>
    /// <param name="id">Идентификатор изделия</param>
    /// <param name="item">Новые данные изделия</param>
    /// <returns>Обновлённое изделие или null, если оно не найдено</returns>
    public Task<ItemDto?> UpdateItemAsync(int id, UpsertItemDto item);

    /// <summary>
    /// Удаляет изделие по идентификатору
    /// Пока на изделие ссылаются заказы, удалить его нельзя
    /// </summary>
    /// <param name="id">Идентификатор изделия</param>
    /// <returns>true, если изделие найдено и удалено</returns>
    public Task<bool> DeleteItemAsync(int id);
}
