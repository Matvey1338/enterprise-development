using System.ComponentModel.DataAnnotations;
using DryCleaning.Application.Exceptions;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using DryCleaning.Domain.Entities;
using DryCleaning.Domain.Interfaces;

namespace DryCleaning.Application.Services;

/// <summary>
/// Операции с принятыми изделиями и преобразование данных в DTO
/// </summary>
public class ItemService(
    IItemRepository itemRepository,
    ICategoryRepository categoryRepository,
    IOrderRepository orderRepository) : IItemService
{
    /// <inheritdoc/>
    public async Task<IReadOnlyList<ItemDto>> GetItemsAsync() =>
        [.. (await itemRepository.GetAllAsync()).Select(MapToDto)];

    /// <inheritdoc/>
    public async Task<ItemDto?> GetItemAsync(int id)
    {
        var item = await itemRepository.GetByIdAsync(id);
        return item is null ? null : MapToDto(item);
    }

    /// <inheritdoc/>
    public async Task<ItemDto> AddItemAsync(UpsertItemDto item) =>
        MapToDto(await itemRepository.AddAsync(await MapToEntityAsync(item)));

    /// <inheritdoc/>
    public async Task<ItemDto?> UpdateItemAsync(int id, UpsertItemDto item)
    {
        if (await itemRepository.GetByIdAsync(id) is null)
        {
            return null;
        }

        var entity = await MapToEntityAsync(item);
        if (!await itemRepository.UpdateAsync(id, entity))
        {
            return null;
        }

        entity.ItemId = id;
        return MapToDto(entity);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteItemAsync(int id)
    {
        if (await orderRepository.HasItemAsync(id))
        {
            throw new EntityInUseException("С изделием связаны заказы — сначала удалите их.");
        }

        return await itemRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Преобразует сущность в DTO
    /// </summary>
    private static ItemDto MapToDto(Item item) => new()
    {
        ItemId = item.ItemId,
        Name = item.Name,
        Material = item.Material,
        CategoryId = item.Category.CategoryId,
        CategoryName = item.Category.Name,
    };

    /// <summary>
    /// Преобразует DTO в сущность с проверкой данных и существования категории
    /// </summary>
    private async Task<Item> MapToEntityAsync(UpsertItemDto dto)
    {
        Validator.ValidateObject(dto, new ValidationContext(dto), validateAllProperties: true);
        return new Item
        {
            ItemId = 0,
            Name = dto.Name,
            Material = dto.Material,
            Category = await categoryRepository.GetByIdAsync(
                    dto.CategoryId ?? throw new ValidationException("Не указана категория изделия."))
                ?? throw new ValidationException("Категория не найдена."),
        };
    }
}
