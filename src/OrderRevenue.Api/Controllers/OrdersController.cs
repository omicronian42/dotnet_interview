using Microsoft.AspNetCore.Mvc;
using OrderRevenue.Api.Dtos;
using OrderRevenue.Api.Services;

namespace OrderRevenue.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;

    public OrdersController(IOrderService orders)
    {
        _orders = orders;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<OrderListItem>>> Get([FromQuery] OrderQuery query, CancellationToken ct)
    {
        return Ok(await _orders.GetOrdersAsync(query, ct));
    }

    // TODO(candidate): PUT /api/orders/{id} - edit status and discount.
    // Two finance users may edit the same order. Decide what happens, and what the client gets back.
    [HttpPut("{id}")]
    public IActionResult Update(string id)
    {
        return StatusCode(StatusCodes.Status501NotImplemented, new ProblemDetails
        {
            Title = "Not implemented",
            Detail = "PUT /api/orders/{id} is part of the exercise. See README.md."
        });
    }
}
