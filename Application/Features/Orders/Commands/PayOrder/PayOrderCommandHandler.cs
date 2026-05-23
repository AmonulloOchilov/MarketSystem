using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Orders.Commands.PayOrder;

public class PayOrderCommandHandler : IRequestHandler<PayOrderCommand, PaymentResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<PayOrderCommandHandler> _logger;

    public PayOrderCommandHandler(IAppDbContext dbContext, ILogger<PayOrderCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<PaymentResponse> Handle(PayOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing payment for Order {OrderId}", request.OrderId);

        var order = await _dbContext.Orders
            .Include(oi => oi.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        
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

        try
        {
            foreach (var item in order.OrderItems)
            {
                var product = await _dbContext.Products.FindAsync(item.ProductId);
                
                if (product == null)
                {
                    throw new ProductNotFoundException(item.ProductId);
                }

                if (product.Stock < item.Quantity)
                {
                    throw new InsufficientStockException(item.ProductId, product.Stock, item.Quantity);
                }
                
                product.Stock -= item.Quantity;
            }

            order.Status = OrderStatus.Paid;
            await _dbContext.SaveChangesAsync(cancellationToken);


            return new PaymentResponse()
            {
                Total = total,
                Paid = request.AmountPaid,
                Change = change
            };
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Error occured while paying order for Customer ID: {CustomerId}",
                order.CustomerId);
            throw;
        }
    }
}