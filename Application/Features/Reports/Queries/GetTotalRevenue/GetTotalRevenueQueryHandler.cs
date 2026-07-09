using Application.DTOs.Response;
using Application.Interfaces.Data;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Reports.Queries.GetTotalRevenue;

public class GetTotalRevenueQueryHandler : IRequestHandler<GetTotalRevenueQuery, TotalRevenueResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetTotalRevenueQueryHandler> _logger;

    public GetTotalRevenueQueryHandler(IAppDbContext dbContext,
        ILogger<GetTotalRevenueQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<TotalRevenueResponse> Handle(GetTotalRevenueQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Orders.AsQueryable();

        if (request.From.HasValue)
        {
            var fromUtc = DateTime.SpecifyKind(request.From.Value, DateTimeKind.Utc);
            query = query.Where(o => o.CreatedAt >= fromUtc);
        }

        if (request.To.HasValue)
        {
            var toUtc = DateTime.SpecifyKind(request.To.Value, DateTimeKind.Utc);
            query = query.Where(o => o.CreatedAt <= toUtc);
        }
        
        query = query.Where(o => o.Status == OrderStatus.Paid);

        var totalOrders = await query.CountAsync(cancellationToken);

        var totalRevenue = await query
            .SelectMany(o => o.OrderItems)
            .SumAsync(oi => oi.Price * oi.Quantity, cancellationToken);

        return new TotalRevenueResponse()
        {
            TotalRevenue = totalRevenue,
            TotalOrders = totalOrders,
            From = request.From,
            To = request.To
        };
    }
}