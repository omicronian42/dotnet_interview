using OrderRevenue.Api.Dtos;

namespace OrderRevenue.Api.Services;

public interface IOrderService
{
    Task<PagedResult<OrderListItem>> GetOrdersAsync(OrderQuery query, CancellationToken ct);

    // TODO(candidate): update an order (status, discount) - and decide how concurrent edits are handled.
    // TODO(candidate): revenue summary - you define the shape.
}
