using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Employees.Queries.GetAllEmployees;

public record GetAllEmployeesQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<EmployeeResponse>>;