using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Reports.Queries.GetEmployeePerformance;

public class GetEmployeePerformanceQueryHandler : IRequestHandler<GetEmployeePerformanceQuery, PagedResponse<EmployeePerformanceResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetEmployeePerformanceQueryHandler> _logger;

    public GetEmployeePerformanceQueryHandler(IAppDbContext dbContext,
        ILogger<GetEmployeePerformanceQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<PagedResponse<EmployeePerformanceResponse>> Handle(GetEmployeePerformanceQuery request,
        CancellationToken cancellationToken)

    {
        _logger.LogInformation("Fetching Employee Reports. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);
        
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);
            return new PagedResponse<EmployeePerformanceResponse>();
        }
        
        var query = _dbContext.Employees
            .Select(e => new EmployeePerformanceResponse()
            {
                Id = e.Id,
                FullName = e.FirstName + " " + e.LastName,
                UserName = e.Username,
                TotalSalesCount = e.Orders.Count,
                TotalRevenueGenerated = e.Orders
                    .SelectMany(o => o.OrderItems)
                    .Sum(oi => oi.Price * oi.Quantity)
            });

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(e => e.TotalRevenueGenerated)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<EmployeePerformanceResponse>()
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}