using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<OrderResponse>
{
    public int OrderId { get; }

    public GetOrderByIdQuery(int orderId)
    {
        OrderId = orderId;
    }
}