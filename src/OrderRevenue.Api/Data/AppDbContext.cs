using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using OrderRevenue.Api.Models;

namespace OrderRevenue.Api.Data;

public class AppDbContext : DbContext
{
    // SQLite hands back DateTimes with Kind=Unspecified. Everything in this app is UTC, so say so.
    private static readonly ValueConverter<DateTime, DateTime> AsUtc =
        new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(x => x.CustomerId);
            e.Property(x => x.Name).IsRequired();
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.ProductId);
            // Dev-database convenience: the SQLite provider cannot ORDER BY / SUM a `decimal` column,
            // so money is stored as REAL here. A production schema would not do this - see README.
            e.Property(x => x.ListPrice).HasConversion<double>();
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.HasKey(x => x.OrderId);
            e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.OrderedAtUtc).HasConversion(AsUtc);
            e.Property(x => x.ShippedAtUtc).HasConversion(AsUtc);
            e.Property(x => x.DiscountAmount).HasConversion<double>();
            e.Property(x => x.OrderTotal).HasConversion<double>();
            e.HasIndex(x => x.OrderedAtUtc);
            e.HasOne(x => x.Customer).WithMany(c => c.Orders).HasForeignKey(x => x.CustomerId);
        });

        modelBuilder.Entity<OrderLine>(e =>
        {
            e.HasKey(x => x.LineId);
            e.Property(x => x.UnitPrice).HasConversion<double>();
            e.HasOne(x => x.Order).WithMany(o => o.Lines).HasForeignKey(x => x.OrderId);
            e.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        });
    }
}
