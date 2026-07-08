using Application.Features.Reports.Queries.GetEmployeePerformance;
using Application.Features.Reports.Queries.GetTotalRevenue;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MarketSystem.API.Controllers;
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ReportsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ReportsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("employees/performance")]
    public async Task<IActionResult> GetEmployeePerformance([FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)

    {
        var result = await _mediator
            .Send(new GetEmployeePerformanceQuery(pageNumber, pageSize));
        return Ok(result);
    }

    [HttpGet("totalrevenue")]
    public async Task<IActionResult> GetTotalRevenue([FromQuery]DateTime? from, [FromQuery] DateTime? to)
    {
        var result = await _mediator.Send(new GetTotalRevenueQuery()
        {
            From = from,
            To = to
        });
        return Ok(result);
    }
}