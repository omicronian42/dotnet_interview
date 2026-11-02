using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using OrderRevenue.Api.Data;
using OrderRevenue.Api.Services;

// Run from the project folder (cd src/OrderRevenue.Api && dotnet run) or from Visual Studio.
// If started from elsewhere, fall back to the project folder so wwwroot and the database are found.
var contentRoot = Directory.GetCurrentDirectory();
if (!Directory.Exists(Path.Combine(contentRoot, "wwwroot")))
{
    var projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));
    if (Directory.Exists(Path.Combine(projectDir, "wwwroot")))
    {
        contentRoot = projectDir;
    }
}

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = contentRoot
});

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "orders.db");
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlite($"Data Source={dbPath}"));
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<SeedLoader>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Schema comes from the model (EnsureCreated) - no migrations. If you change the model,
    // delete orders.db and restart; the CSVs are re-loaded automatically.
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
    await scope.ServiceProvider.GetRequiredService<SeedLoader>().LoadIfEmptyAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();

app.Run();
