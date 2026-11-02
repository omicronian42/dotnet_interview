namespace OrderRevenue.Api.Models;

public class Customer
{
    public string CustomerId { get; set; } = "";
    public string Name { get; set; } = "";
    public string Gstin { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string Region { get; set; } = "";
    public DateTime CreatedAt { get; set; }

    public List<Order> Orders { get; set; } = new();
}
