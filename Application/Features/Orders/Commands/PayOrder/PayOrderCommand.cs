using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Orders.Commands.PayOrder;

public class PayOrderCommand : IRequest<PaymentResponse>
{
    public int OrderId { get; }
    public decimal AmountPaid { get; }

    public PayOrderCommand(int orderId, decimal amountPaid)
    {
        OrderId = orderId;
        AmountPaid = amountPaid;
    }
}