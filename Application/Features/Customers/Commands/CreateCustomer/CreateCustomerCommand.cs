using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Customers.Commands.CreateCustomer;

public record CreateCustomerCommand(CreateCustomerRequest Request) : IRequest<CustomerResponse>;