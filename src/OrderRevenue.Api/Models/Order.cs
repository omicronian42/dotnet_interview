namespace OrderRevenue.Api.Models;

public class Order
{
    public string OrderId { get; set; } = "";
    public string CustomerId { get; set; } = "";
    public Customer? Customer { get; set; }

    /// <summary>All timestamps are stored and exposed in UTC.</summary>
    public DateTime OrderedAtUtc { get; set; }
    public DateTime? ShippedAtUtc { get; set; }

    public OrderStatus Status { get; set; }

    /// <summary>Order-level discount in INR.</summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>Total recorded on the order header, INR, excl. GST.</summary>
    public decimal OrderTotal { get; set; }

    public List<OrderLine> Lines { get; set; } = new();
}
