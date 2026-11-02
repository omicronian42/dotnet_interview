namespace OrderRevenue.Api.Models;

public class Product
{
    public string ProductId { get; set; } = "";
    public string Sku { get; set; } = "";
    public string Name { get; set; } = "";
    public string Category { get; set; } = "";

    /// <summary>Current list price (INR). Not necessarily the price a past order was placed at.</summary>
    public decimal ListPrice { get; set; }
}
