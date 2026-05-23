using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(CreateOrderRequest Request) : IRequest<OrderResponse>;