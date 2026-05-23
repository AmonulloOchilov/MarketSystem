using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PagedResponse<OrderResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetAllOrdersQueryHandler> _logger;

    public GetAllOrdersQueryHandler(IAppDbContext dbContext, ILogger<GetAllOrdersQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<PagedResponse<OrderResponse>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching orders. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);
        
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);
            return new PagedResponse<OrderResponse>();
        }

        var query = _dbContext.Orders
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);

        var orders = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);
        
        var items = orders.Select(o => new OrderResponse()
        {
            Id = o.Id,
            CustomerId = o.CustomerId,
            EmployeeId = o.EmployeeId,
            CreatedAt = o.CreatedAt,
            Status = o.Status,
            Items = o.OrderItems.Select(i => new OrderItemResponse()
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,
                TotalPrice = i.Quantity * i.Price
            }).ToList()

        }).ToList();

        _logger.LogInformation("Returned {Count} orders out of {Total}", items.Count, totalCount);
        
        return new PagedResponse<OrderResponse>()
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}