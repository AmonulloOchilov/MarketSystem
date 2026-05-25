using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Employees.Commands.CreateEmployee;

public record CreateEmployeeCommand(CreateEmployeeRequest Request) : IRequest<EmployeeResponse>;