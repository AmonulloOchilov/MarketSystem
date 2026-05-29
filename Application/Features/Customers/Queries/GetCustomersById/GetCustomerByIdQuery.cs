using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Customers.Queries.GetCustomersById;

public record GetCustomerByIdQuery(int CustomerId) : IRequest<CustomerResponse>;
