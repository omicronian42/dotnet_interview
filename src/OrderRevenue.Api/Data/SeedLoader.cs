using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using OrderRevenue.Api.Models;

namespace OrderRevenue.Api.Data;

/// <summary>
/// Loads the CSV files in /data into the SQLite database on first run.
/// Rules (kept deliberately simple):
///  - rows whose primary key was already loaded are skipped (and counted in the log)
///  - order status text is trimmed and parsed case-insensitively into the enum
///  - timestamps are parsed as UTC
/// Delete orders.db to force a re-seed.
/// </summary>
public class SeedLoader
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly IHostEnvironment _env;
    private readonly ILogger<SeedLoader> _log;

    public SeedLoader(AppDbContext db, IConfiguration config, IHostEnvironment env, ILogger<SeedLoader> log)
    {
        _db = db;
        _config = config;
        _env = env;
        _log = log;
    }

    public async Task LoadIfEmptyAsync()
    {
        if (await _db.Orders.AnyAsync())
        {
            _log.LogInformation("Database already seeded - skipping CSV load.");
            return;
        }

        var dataPath = ResolveDataPath();
        _log.LogInformation("Seeding from {DataPath}", dataPath);

        foreach (var r in Read<CustomerRow>(dataPath, "customers.csv"))
        {
            _db.Customers.Add(new Customer
            {
                CustomerId = r.CustomerId,
                Name = r.Name,
                Gstin = r.Gstin,
                City = r.City,
                State = r.State,
                Region = r.Region,
                CreatedAt = DateTime.Parse(r.CreatedAt, CultureInfo.InvariantCulture)
            });
        }

        foreach (var r in Read<ProductRow>(dataPath, "products.csv"))
        {
            _db.Products.Add(new Product
            {
                ProductId = r.ProductId,
                Sku = r.Sku,
                Name = r.Name,
                Category = r.Category,
                ListPrice = decimal.Parse(r.ListPrice, CultureInfo.InvariantCulture)
            });
        }

        var orderIds = new HashSet<string>();
        var skippedOrders = 0;
        foreach (var r in Read<OrderRow>(dataPath, "orders.csv"))
        {
            if (!orderIds.Add(r.OrderId))
            {
                skippedOrders++;
                continue;
            }

            _db.Orders.Add(new Order
            {
                OrderId = r.OrderId,
                CustomerId = r.CustomerId,
                OrderedAtUtc = ParseUtc(r.OrderTsUtc),
                ShippedAtUtc = string.IsNullOrWhiteSpace(r.ShipTsUtc) ? null : ParseUtc(r.ShipTsUtc),
                Status = Enum.Parse<OrderStatus>(r.Status.Trim(), ignoreCase: true),
                DiscountAmount = decimal.Parse(r.DiscountAmount, CultureInfo.InvariantCulture),
                OrderTotal = decimal.Parse(r.OrderTotal, CultureInfo.InvariantCulture)
            });
        }

        var lineIds = new HashSet<string>();
        var skippedLines = 0;
        foreach (var r in Read<LineRow>(dataPath, "order_lines.csv"))
        {
            if (!lineIds.Add(r.LineId))
            {
                skippedLines++;
                continue;
            }

            _db.OrderLines.Add(new OrderLine
            {
                LineId = r.LineId,
                OrderId = r.OrderId,
                ProductId = r.ProductId,
                Quantity = int.Parse(r.Quantity, CultureInfo.InvariantCulture),
                UnitPrice = decimal.Parse(r.UnitPrice, CultureInfo.InvariantCulture),
                ReturnedQty = int.Parse(r.ReturnedQty, CultureInfo.InvariantCulture)
            });
        }

        await _db.SaveChangesAsync();

        _log.LogInformation(
            "Seeded {Orders} orders ({SkippedOrders} duplicate order rows skipped) and {Lines} order lines ({SkippedLines} duplicate line rows skipped).",
            orderIds.Count, skippedOrders, lineIds.Count, skippedLines);
    }

    private static DateTime ParseUtc(string value) =>
        DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).UtcDateTime;

    private static List<T> Read<T>(string dataPath, string fileName)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true };
        using var reader = new StreamReader(Path.Combine(dataPath, fileName));
        using var csv = new CsvReader(reader, config);
        return csv.GetRecords<T>().ToList();
    }

    private string ResolveDataPath()
    {
        var configured = _config["Seed:DataPath"];
        if (!string.IsNullOrWhiteSpace(configured))
        {
            return Path.GetFullPath(configured);
        }

        // Walk up from the content root and from the build output folder until we find data/orders.csv.
        foreach (var start in new[] { _env.ContentRootPath, AppContext.BaseDirectory })
        {
            var dir = new DirectoryInfo(start);
            for (var i = 0; i < 8 && dir != null; i++, dir = dir.Parent)
            {
                var candidate = Path.Combine(dir.FullName, "data");
                if (File.Exists(Path.Combine(candidate, "orders.csv")))
                {
                    return candidate;
                }
            }
        }

        throw new DirectoryNotFoundException(
            "Could not find the data folder (data/orders.csv). Set Seed:DataPath in appsettings.json.");
    }

    // ---- CSV row shapes (strings only; parsing is explicit above) ----
    internal sealed class CustomerRow
    {
        [Name("customer_id")] public string CustomerId { get; set; } = "";
        [Name("name")] public string Name { get; set; } = "";
        [Name("gstin")] public string Gstin { get; set; } = "";
        [Name("city")] public string City { get; set; } = "";
        [Name("state")] public string State { get; set; } = "";
        [Name("region")] public string Region { get; set; } = "";
        [Name("created_at")] public string CreatedAt { get; set; } = "";
    }

    internal sealed class ProductRow
    {
        [Name("product_id")] public string ProductId { get; set; } = "";
        [Name("sku")] public string Sku { get; set; } = "";
        [Name("name")] public string Name { get; set; } = "";
        [Name("category")] public string Category { get; set; } = "";
        [Name("list_price")] public string ListPrice { get; set; } = "";
    }

    internal sealed class OrderRow
    {
        [Name("order_id")] public string OrderId { get; set; } = "";
        [Name("customer_id")] public string CustomerId { get; set; } = "";
        [Name("order_ts_utc")] public string OrderTsUtc { get; set; } = "";
        [Name("ship_ts_utc")] public string ShipTsUtc { get; set; } = "";
        [Name("status")] public string Status { get; set; } = "";
        [Name("discount_amount")] public string DiscountAmount { get; set; } = "";
        [Name("order_total")] public string OrderTotal { get; set; } = "";
    }

    internal sealed class LineRow
    {
        [Name("line_id")] public string LineId { get; set; } = "";
        [Name("order_id")] public string OrderId { get; set; } = "";
        [Name("product_id")] public string ProductId { get; set; } = "";
        [Name("quantity")] public string Quantity { get; set; } = "";
        [Name("unit_price")] public string UnitPrice { get; set; } = "";
        [Name("returned_qty")] public string ReturnedQty { get; set; } = "";
    }
}
