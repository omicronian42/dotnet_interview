namespace OrderRevenue.Api.Models;

public class OrderLine
{
    public string LineId { get; set; } = "";
    public string OrderId { get; set; } = "";
    public Order? Order { get; set; }

    public string ProductId { get; set; } = "";
    public Product? Product { get; set; }

    public int Quantity { get; set; }

    /// <summary>Price per unit at the time of the order, INR, excl. GST.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Units the customer sent back (0 if none).</summary>
    public int ReturnedQty { get; set; }
}
