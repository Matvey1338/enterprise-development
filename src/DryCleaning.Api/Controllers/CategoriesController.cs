using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DryCleaning.Api.Controllers;

/// <summary>
/// CRUD-операции и запросы для сущностей Category
/// </summary>
/// <param name="service">Сервис категорий</param>
[ApiController]
[Route("api/categories")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
public class CategoriesController(ICategoryService service) : ControllerBase
{
    /// <summary>
    /// Возвращает все категории
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<CategoryDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories() =>
        Ok(await service.GetCategoriesAsync());

    /// <summary>
    /// Возвращает категорию по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id)
    {
        var category = await service.GetCategoryAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    /// <summary>
    /// Создаёт категорию; идентификатор присваивается сервером
    /// </summary>
    [HttpPost]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> AddCategory(UpsertCategoryDto category)
    {
        var created = await service.AddCategoryAsync(category);
        return CreatedAtAction(nameof(GetCategory), new { id = created.CategoryId }, created);
    }

    /// <summary>
    /// Обновляет категорию и возвращает обновлённые данные
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<CategoryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(int id, UpsertCategoryDto category)
    {
        var updated = await service.UpdateCategoryAsync(id, category);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Удаляет категорию по идентификатору
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteCategory(int id) =>
        await service.DeleteCategoryAsync(id) ? NoContent() : NotFound();

    /// <summary>
    /// Возвращает топ наиболее популярных категорий изделий за последний год
    /// </summary>
    [HttpGet("most-popular")]
    [ProducesResponseType<IReadOnlyList<CategoryPopularityDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<CategoryPopularityDto>>> GetMostPopularCategories(
        [FromQuery, Range(1, int.MaxValue)] int count = 5) =>
        Ok(await service.GetMostPopularCategoriesAsync(count));

    /// <summary>
    /// Возвращает топ наименее популярных категорий изделий за последний год
    /// </summary>
    [HttpGet("least-popular")]
    [ProducesResponseType<IReadOnlyList<CategoryPopularityDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<CategoryPopularityDto>>> GetLeastPopularCategories(
        [FromQuery, Range(1, int.MaxValue)] int count = 5) =>
        Ok(await service.GetLeastPopularCategoriesAsync(count));
}
