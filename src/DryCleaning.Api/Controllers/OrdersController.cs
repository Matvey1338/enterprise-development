using DryCleaning.Contracts.Dto;
using DryCleaning.Contracts.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DryCleaning.Api.Controllers;

/// <summary>
/// CRUD-операции и запросы для сущностей Order
/// </summary>
/// <param name="service">Сервис заказов</param>
[ApiController]
[Route("api/orders")]
[ProducesResponseType<ProblemDetails>(StatusCodes.Status500InternalServerError)]
public class OrdersController(IOrderService service) : ControllerBase
{
    /// <summary>
    /// Возвращает все заказы
    /// </summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<OrderDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrders() =>
        Ok(await service.GetOrdersAsync());

    /// <summary>
    /// Возвращает заказ по идентификатору
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<OrderDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> GetOrder(int id)
    {
        var order = await service.GetOrderAsync(id);
        return order is null ? NotFound() : Ok(order);
    }

    /// <summary>
    /// Создаёт заказ; идентификатор присваивается сервером
    /// </summary>
    [HttpPost]
    [ProducesResponseType<OrderDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrderDto>> AddOrder(UpsertOrderDto order)
    {
        var created = await service.AddOrderAsync(order);
        return CreatedAtAction(nameof(GetOrder), new { id = created.OrderId }, created);
    }

    /// <summary>
    /// Обновляет заказ и возвращает обновлённые данные
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<OrderDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderDto>> UpdateOrder(int id, UpsertOrderDto order)
    {
        var updated = await service.UpdateOrderAsync(id, order);
        return updated is null ? NotFound() : Ok(updated);
    }

    /// <summary>
    /// Удаляет заказ по идентификатору
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteOrder(int id) =>
        await service.DeleteOrderAsync(id) ? NoContent() : NotFound();

    /// <summary>
    /// Возвращает заказы, находящиеся в обработке, упорядоченные по дате приёма
    /// </summary>
    [HttpGet("in-processing")]
    [ProducesResponseType<IReadOnlyList<OrderDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetOrdersInProcessing() =>
        Ok(await service.GetInProcessingAsync());
}
