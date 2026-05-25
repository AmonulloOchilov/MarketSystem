using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Employees.Commands.UpdateEmployee;

public record UpdateEmployeeCommand(int EmployeeId, UpdateEmployeeRequest Request) : IRequest<EmployeeResponse>;
