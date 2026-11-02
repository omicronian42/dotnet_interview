using OrderRevenue.Api.Models;

namespace OrderRevenue.Api.Dtos;

public record OrderListItem(
    string OrderId,
    string CustomerId,
    string CustomerName,
    DateTime OrderedAtUtc,
    DateTime? ShippedAtUtc,
    OrderStatus Status,
    decimal DiscountAmount,
    decimal OrderTotal);
