using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<OrderResponse>
{
    public CreateOrderRequest Request { get; }
    
    public CreateOrderCommand(CreateOrderRequest request)
    {
        Request = request;
    }
}