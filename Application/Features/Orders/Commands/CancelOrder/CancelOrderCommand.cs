using MediatR;

namespace Application.Features.Orders.Commands.CancelOrder;

public record CancelOrderCommand(int OrderId) : IRequest;