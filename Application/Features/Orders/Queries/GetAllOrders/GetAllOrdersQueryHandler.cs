using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, PagedResponse<OrderResponse>>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<GetAllOrdersQueryHandler> _logger;

    public GetAllOrdersQueryHandler(IOrderRepository repository, ILogger<GetAllOrdersQueryHandler> logger)
    {
        _repository = repository;
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

        var orders = await _repository.GetAllAsync(request.PageNumber, request.PageSize);
        
        var items = orders.Items.Select(o => new OrderResponse()
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

        _logger.LogInformation("Returned {Count} orders out of {Total}", items.Count, orders.TotalCount);
        
        return new PagedResponse<OrderResponse>()
        {
            Items = items,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = orders.TotalCount
        };
    }
}