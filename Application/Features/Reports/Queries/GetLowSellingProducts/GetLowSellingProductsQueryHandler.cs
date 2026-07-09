using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Reports.Queries.GetLowSellingProducts;

public class GetLowSellingProductsQueryHandler : IRequestHandler<GetLowSellingProductsQuery, PagedResponse<LowSellingProductsResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetLowSellingProductsQueryHandler> _logger;

    public GetLowSellingProductsQueryHandler(IAppDbContext dbContext,
        ILogger<GetLowSellingProductsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<PagedResponse<LowSellingProductsResponse>> Handle(GetLowSellingProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching Low Selling Products Report. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);
        
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);
            return new PagedResponse<LowSellingProductsResponse>();
        }
        
        var query = _dbContext.OrderItems
            .GroupBy(oi => new
            {
                oi.ProductId,
                oi.Product.Name,
                oi.Price
            })
            .Select(g => new LowSellingProductsResponse()
            {
                Id = g.Key.ProductId,
                ProductName = g.Key.Name,
                Price = g.Key.Price,
                SoldQuantity = g.Sum(x => x.Quantity),
                TotalRevenue = g.Sum(x => x.Quantity * x.Price)
            });

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(i => i.SoldQuantity)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResponse<LowSellingProductsResponse>()
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}