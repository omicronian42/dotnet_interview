using OrderRevenue.Api.Services;
using Xunit;

namespace OrderRevenue.Tests;

// A deliberately tiny example so the test project is wired up and runs.
// Tests are optional for this exercise - a few focused ones on your revenue logic are worth more than many shallow ones.
public class PagingTests
{
    [Theory]
    [InlineData(0, 0, 1, 25)]
    [InlineData(3, 10, 3, 10)]
    [InlineData(2, 5000, 2, 100)]
    public void Normalize_ClampsToSafeValues(int page, int pageSize, int expectedPage, int expectedPageSize)
    {
        var (p, s) = Paging.Normalize(page, pageSize);

        Assert.Equal(expectedPage, p);
        Assert.Equal(expectedPageSize, s);
    }
}
