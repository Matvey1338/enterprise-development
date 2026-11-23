using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DryCleaning.Api.Controllers;

/// <summary>
/// CRUD-операции для сущностей Item
/// </summary>
/// <param name="service">Сервис изделий</param>
[ApiController]
[Route("api/items")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
public class ItemsController(IItemService service) : ControllerBase
{
    /// <summary>
    /// Возвращает все принятые изделия
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ItemDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ItemDto>>> GetItems() =>
        Ok(await service.GetItemsAsync());

    /// <summary>
    /// Возвращает изделие по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemDto>> GetItem(int id)
    {
        var item = await service.GetItemAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    /// <summary>
    /// Создаёт изделие; идентификатор присваивается сервером
    /// </summary>
    [HttpPost]
    [ProducesResponseType<ItemDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemDto>> AddItem(UpsertItemDto item)
    {
        var created = await service.AddItemAsync(item);
        return CreatedAtAction(nameof(GetItem), new { id = created.ItemId }, created);
    }

    /// <summary>
    /// Обновляет изделие и возвращает обновлённые данные
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<ItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemDto>> UpdateItem(int id, UpsertItemDto item)
    {
        var updated = await service.UpdateItemAsync(id, item);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Удаляет изделие по идентификатору
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteItem(int id) =>
        await service.DeleteItemAsync(id) ? NoContent() : NotFound();
}
