namespace OrderRevenue.Api.Dtos;

/// <summary>Query-string parameters for GET /api/orders.</summary>
public class OrderQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;

    /// <summary>Order id or customer name. (Not wired up yet.)</summary>
    public string? Search { get; set; }

    /// <summary>orderedAtUtc | orderTotal | customerName | status. (Not wired up yet.)</summary>
    public string? SortBy { get; set; }

    /// <summary>asc | desc. (Not wired up yet.)</summary>
    public string? SortDir { get; set; }
}
