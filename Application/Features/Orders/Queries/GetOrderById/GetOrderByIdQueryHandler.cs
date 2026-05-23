using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, OrderResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetOrderByIdQueryHandler> _logger;

    public GetOrderByIdQueryHandler(IAppDbContext dbContext, ILogger<GetOrderByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<OrderResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.Include(oi => oi.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            _logger.LogWarning("Order with ID {OrderId} not found", request.OrderId);
            throw new OrderNotFoundException(request.OrderId);
        }

        return new OrderResponse
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            EmployeeId = order.EmployeeId,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Items = order.OrderItems.Select(i => new OrderItemResponse
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                Price = i.Price,
                TotalPrice = i.Quantity * i.Price
            }).ToList()
        };
    }
}