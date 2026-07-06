using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Customers.Commands.DeleteCustomer;

public record DeleteCustomerCommand(int CustomerId) : IRequest, IRequest<bool>;
