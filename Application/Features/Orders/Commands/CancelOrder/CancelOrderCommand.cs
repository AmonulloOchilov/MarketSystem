using MediatR;

namespace Application.Features.Orders.Commands.CancelOrder;

public class CancelOrderCommand : IRequest
{
    public int OrderId { get; }

    public CancelOrderCommand(int orderId)
    {
        OrderId = orderId;
    }
}