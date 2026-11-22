using DryCleaning.Contracts.Dto;

namespace DryCleaning.Contracts.Interfaces;

/// <summary>
/// Сервис категорий изделий химчистки
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Возвращает список всех категорий (справочник)
    /// </summary>
    public Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync();

    /// <summary>
    /// Возвращает категорию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    public Task<CategoryDto?> GetCategoryAsync(int id);

    /// <summary>
    /// Добавляет категорию и возвращает созданную сущность с присвоенным идентификатором
    /// </summary>
    /// <param name="category">Данные категории</param>
    public Task<CategoryDto> AddCategoryAsync(UpsertCategoryDto category);

    /// <summary>
    /// Заменяет категорию с указанным идентификатором
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <param name="category">Новые данные категории</param>
    /// <returns>Обновлённая категория или null, если она не найдена</returns>
    public Task<CategoryDto?> UpdateCategoryAsync(int id, UpsertCategoryDto category);

    /// <summary>
    /// Удаляет категорию по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор категории</param>
    /// <returns>true, если категория найдена и удалена</returns>
    public Task<bool> DeleteCategoryAsync(int id);

    /// <summary>
    /// Возвращает топ наиболее популярных категорий изделий за последний год:
    /// категории упорядочены по убыванию числа заказов,
    /// при равном числе заказов — по названию
    /// </summary>
    /// <param name="count">Количество категорий в результате</param>
    public Task<IReadOnlyList<CategoryPopularityDto>> GetMostPopularCategoriesAsync(int count = 5);

    /// <summary>
    /// Возвращает топ наименее популярных категорий изделий за последний год:
    /// категории упорядочены по возрастанию числа заказов
    /// (категории без заказов считаются наименее популярными),
    /// при равном числе заказов — по названию
    /// </summary>
    /// <param name="count">Количество категорий в результате</param>
    public Task<IReadOnlyList<CategoryPopularityDto>> GetLeastPopularCategoriesAsync(int count = 5);
}
