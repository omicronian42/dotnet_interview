using Microsoft.AspNetCore.Mvc;

namespace OrderRevenue.Api.Controllers;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase
{
    // TODO(candidate): monthly revenue + top 5 customers. The response shape and the definition of
    // "revenue" are yours to decide - write them down in DECISIONS.md.
    [HttpGet("revenue-summary")]
    public IActionResult RevenueSummary()
    {
        return StatusCode(StatusCodes.Status501NotImplemented, new ProblemDetails
        {
            Title = "Not implemented",
            Detail = "GET /api/reports/revenue-summary is part of the exercise. See README.md."
        });
    }
}
