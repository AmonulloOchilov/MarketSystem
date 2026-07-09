using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Reports.Queries.GetTopSellingProducts;

public class GetTopSellingProductsQueryHandler : IRequestHandler<GetTopSellingProductsQuery, PagedResponse<TopSellingProductsResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetTopSellingProductsQueryHandler> _logger;

    public GetTopSellingProductsQueryHandler(IAppDbContext dbContext, ILogger<GetTopSellingProductsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<PagedResponse<TopSellingProductsResponse>> Handle(GetTopSellingProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching Top Selling Products Report. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);
        
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);
            return new PagedResponse<TopSellingProductsResponse>();
        }
        
        var query = _dbContext.OrderItems
            .GroupBy(oi => new
            {
                oi.ProductId,
                oi.Product.Name,
                oi.Price
            })
            .Select(g => new TopSellingProductsResponse()
            {
                Id = g.Key.ProductId,
                ProductName = g.Key.Name,
                Price = g.Key.Price,
                SoldQuantity = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.Price)
            });

        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .OrderByDescending(x => x.SoldQuantity)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        return new PagedResponse<TopSellingProductsResponse>()
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}