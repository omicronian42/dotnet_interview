using Microsoft.EntityFrameworkCore;
using OrderRevenue.Api.Data;
using OrderRevenue.Api.Dtos;
using OrderRevenue.Api.Models;

namespace OrderRevenue.Api.Services;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;

    public OrderService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<OrderListItem>> GetOrdersAsync(OrderQuery query, CancellationToken ct)
    {
        var (page, pageSize) = Paging.Normalize(query.Page, query.PageSize);

        IQueryable<Order> orders = _db.Orders.AsNoTracking();

        // TODO(candidate): apply query.Search (order id or customer name).

        // TODO(candidate): apply query.SortBy / query.SortDir. Keep a tie-breaker so paging is stable.
        orders = orders.OrderByDescending(o => o.OrderedAtUtc).ThenBy(o => o.OrderId);

        var total = await orders.CountAsync(ct);

        var items = await orders
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(o => new OrderListItem(
                o.OrderId,
                o.CustomerId,
                o.Customer!.Name,
                o.OrderedAtUtc,
                o.ShippedAtUtc,
                o.Status,
                o.DiscountAmount,
                o.OrderTotal))
            .ToListAsync(ct);

        return new PagedResult<OrderListItem>(items, total, page, pageSize);
    }
}
