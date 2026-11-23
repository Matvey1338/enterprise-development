using System.ComponentModel.DataAnnotations;
using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DryCleaning.Api.Controllers;

/// <summary>
/// CRUD-операции и запросы для сущностей Client
/// </summary>
/// <param name="service">Сервис клиентов</param>
[ApiController]
[Route("api/clients")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
public class ClientsController(IClientService service) : ControllerBase
{
    /// <summary>
    /// Возвращает всех клиентов
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ClientDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClientDto>>> GetClients() =>
        Ok(await service.GetClientsAsync());

    /// <summary>
    /// Возвращает клиента по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<ClientDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientDto>> GetClient(int id)
    {
        var client = await service.GetClientAsync(id);
        return client is null ? NotFound() : Ok(client);
    }

    /// <summary>
    /// Создаёт клиента; идентификатор присваивается сервером
    /// </summary>
    [HttpPost]
    [ProducesResponseType<ClientDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClientDto>> AddClient(UpsertClientDto client)
    {
        var created = await service.AddClientAsync(client);
        return CreatedAtAction(nameof(GetClient), new { id = created.ClientId }, created);
    }

    /// <summary>
    /// Обновляет клиента и возвращает обновлённые данные
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<ClientDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientDto>> UpdateClient(int id, UpsertClientDto client)
    {
        var updated = await service.UpdateClientAsync(id, client);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Удаляет клиента по идентификатору
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteClient(int id) =>
        await service.DeleteClientAsync(id) ? NoContent() : NotFound();

    /// <summary>
    /// Возвращает топ клиентов по числу сданных изделий за период [from; to]
    /// </summary>
    [HttpGet("top-by-items")]
    [ProducesResponseType<IReadOnlyList<ClientItemsCountDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<ClientItemsCountDto>>> GetTopClientsByItems(
        [FromQuery] DateOnly from, [FromQuery] DateOnly to,
        [FromQuery, Range(1, int.MaxValue)] int count = 5) =>
        Ok(await service.GetTopClientsByItemsCountAsync(from, to, count));

    /// <summary>
    /// Возвращает клиентов, чьи заказы имеют максимальный срок выполнения
    /// </summary>
    [HttpGet("longest-processing")]
    [ProducesResponseType<IReadOnlyList<ClientDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ClientDto>>> GetClientsWithLongestProcessing() =>
        Ok(await service.GetClientsWithLongestProcessingAsync());

    /// <summary>
    /// Возвращает клиента с наибольшей суммой трат за весь период
    /// </summary>
    [HttpGet("top-spender")]
    [ProducesResponseType<ClientSpendingDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClientSpendingDto>> GetTopSpender()
    {
        var topSpender = await service.GetTopSpenderAsync();
        return topSpender is null ? NotFound() : Ok(topSpender);
    }
}
