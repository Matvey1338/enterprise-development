using System.ComponentModel.DataAnnotations;
using DryCleaning.Application.Exceptions;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Application.Services;

/// <summary>
/// Операции с категориями изделий и преобразование данных в DTO
/// </summary>
public class CategoryService(
    ICategoryRepository categoryRepository,
    IItemRepository itemRepository,
    IOrderRepository orderRepository,
    Func<DateTime>? clock = null) : ICategoryService
{
    private readonly Func<DateTime> _clock = clock ?? (() => DateTime.Now);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync() =>
        [.. (await categoryRepository.GetAllAsync()).Select(MapToDto)];

    /// <inheritdoc/>
    public async Task<CategoryDto?> GetCategoryAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id);
        return category is null ? null : MapToDto(category);
    }

    /// <inheritdoc/>
    public async Task<CategoryDto> AddCategoryAsync(UpsertCategoryDto category) =>
        MapToDto(await categoryRepository.AddAsync(MapToEntity(category)));

    /// <inheritdoc/>
    public async Task<CategoryDto?> UpdateCategoryAsync(int id, UpsertCategoryDto category)
    {
        if (await categoryRepository.GetByIdAsync(id) is null)
        {
            return null;
        }

        var entity = MapToEntity(category);
        if (!await categoryRepository.UpdateAsync(id, entity))
        {
            return null;
        }

        entity.CategoryId = id;
        return MapToDto(entity);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        if (await itemRepository.HasCategoryAsync(id))
        {
            throw new EntityInUseException("С категорией связаны принятые изделия — сначала удалите их.");
        }

        return await categoryRepository.DeleteAsync(id);
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CategoryPopularityDto>> GetMostPopularCategoriesAsync(int count = 5) =>
        [.. (await GetPopularityAsync())
            .OrderByDescending(x => x.OrdersCount)
            .ThenBy(x => x.Name, StringComparer.Ordinal)
            .Take(count)];

    /// <inheritdoc/>
    public async Task<IReadOnlyList<CategoryPopularityDto>> GetLeastPopularCategoriesAsync(int count = 5) =>
        [.. (await GetPopularityAsync())
            .OrderBy(x => x.OrdersCount)
            .ThenBy(x => x.Name, StringComparer.Ordinal)
            .Take(count)];

    /// <summary>
    /// Возвращает популярность всех категорий за последний год:
    /// категория и число заказов на её изделия.
    /// Категории без заказов за период имеют нулевую популярность
    /// </summary>
    private async Task<IReadOnlyList<CategoryPopularityDto>> GetPopularityAsync()
    {
        var since = DateOnly.FromDateTime(_clock()).AddYears(-1);
        var ordersCounts = await orderRepository.GetCategoryOrdersCountsAsync(since);
        var categories = await categoryRepository.GetAllAsync();

        return [.. categories.Select(category => new CategoryPopularityDto
        {
            CategoryId = category.CategoryId,
            Name = category.Name,
            OrdersCount = ordersCounts.GetValueOrDefault(category.CategoryId),
        })];
    }

    /// <summary>
    /// Преобразует сущность в DTO
    /// </summary>
    private static CategoryDto MapToDto(Category category) => new()
    {
        CategoryId = category.CategoryId,
        Name = category.Name,
        RecommendedCleaningType = category.RecommendedCleaningType,
        CleaningPrice = category.CleaningPrice,
    };

    /// <summary>
    /// Преобразует DTO в сущность с проверкой данных
    /// </summary>
    private static Category MapToEntity(UpsertCategoryDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        return new Category
        {
            CategoryId = 0,
            Name = dto.Name,
            RecommendedCleaningType = dto.RecommendedCleaningType
                ?? throw new ValidationException("Рекомендуемый вид чистки обязателен."),
            CleaningPrice = dto.CleaningPrice
                ?? throw new ValidationException("Стоимость чистки обязательна."),
        };
    }
}
