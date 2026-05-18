using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Persistence;
using Application.Interfaces.Repositories;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.PayOrder;

public class PayOrderCommandHandler : IRequestHandler<PayOrderCommand, PaymentResponse>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<PayOrderCommandHandler> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PayOrderCommandHandler(IOrderRepository orderRepository, IProductRepository productRepository, ILogger<PayOrderCommandHandler> logger, IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _productRepository = productRepository;
    }
    public async Task<PaymentResponse> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment for Order {OrderId}", request.OrderId);
        
        var order = await _orderRepository.GetByIdAsync(request.OrderId);
        
        if (order == null)
        {
            throw new OrderNotFoundException(request.OrderId);
        }

        if (order.Status == OrderStatus.Paid)
        {
            throw new OrderAlreadyPaidException(request.OrderId);
        }
        
        var total = order.OrderItems.Sum(i => i.Price * i.Quantity);
        if (request.AmountPaid < total)
        {
            throw new InsufficientPaymentException(request.AmountPaid, total);
        }

        var change = request.AmountPaid - total;

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                
                if (product == null)
                {
                    throw new ProductNotFoundException(item.ProductId);
                }

                if (product.Stock < item.Quantity)
                {
                    throw new InsufficientStockException(item.ProductId, product.Stock, item.Quantity);
                }
                
                product.Stock -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            order.Status = OrderStatus.Paid;
            await _orderRepository.UpdateAsync(order);

            await _unitOfWork.CommitAsync();

            return new PaymentResponse()
            {
                Total = total,
                Paid = request.AmountPaid,
                Change = change
            };
        }
        catch
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}