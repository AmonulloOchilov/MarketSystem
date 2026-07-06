using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Customers.Queries.GetAllCustomers;

public record GetAllCustomersQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<CustomerResponse>>;