using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Orders.Commands.PayOrder;

public record PayOrderCommand(int OrderId, decimal AmountPaid) : IRequest<PaymentResponse>;