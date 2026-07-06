using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Customers.Commands.UpdateCustomer;

public record UpdateCustomerCommand(int CustomerId, UpdateCustomerRequest Request) : IRequest<CustomerResponse>;
