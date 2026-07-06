using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Orders.Queries.GetOrderById;

public record GetOrderByIdQuery(int OrderId) : IRequest<OrderResponse>;