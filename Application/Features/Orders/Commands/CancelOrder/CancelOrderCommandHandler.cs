using Application.Exceptions;
using Application.Interfaces.Data;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<CancelOrderCommandHandler> _logger;

    public CancelOrderCommandHandler(IAppDbContext dbContext, ILogger<CancelOrderCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _dbContext.Orders.FindAsync(request.OrderId);
        
        if (order == null)
        {
            throw new OrderNotFoundException(request.OrderId);
        }

        if (order.Status == OrderStatus.Paid)
        {
            throw new OrderAlreadyPaidException(request.OrderId);
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new InvalidOrderException($"Order {request.OrderId} is already cancelled");
        }
        
        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOrderException("Only pending orders can be cancelled");
        }

        try
        {
            _logger.LogInformation("Cancelling Order {OrderId}", request.OrderId);
            
            order.Status = OrderStatus.Cancelled;
            
            await _dbContext.SaveChangesAsync(cancellationToken);

        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occured while cancelling order for Order ID: {OrderId}",
                request.OrderId);
            throw;
        }
    }
}