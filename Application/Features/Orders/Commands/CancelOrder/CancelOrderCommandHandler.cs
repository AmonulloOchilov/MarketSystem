using Application.Exceptions;
using Application.Interfaces.Persistence;
using Application.Interfaces.Repositories;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<CancelOrderCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CancelOrderCommandHandler(IOrderRepository orderRepository, ILogger<CancelOrderCommandHandler> logger,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        
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

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _logger.LogInformation("Cancelling Order {OrderId}", request.OrderId);
            
            order.Status = OrderStatus.Cancelled;
            await _orderRepository.UpdateAsync(order);

            await _unitOfWork.CommitAsync();
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}