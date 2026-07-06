using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Employees.Queries.GetEmployeeById;

public record GetEmployeeByIdQuery(int EmployeeId) : IRequest<EmployeeResponse>;