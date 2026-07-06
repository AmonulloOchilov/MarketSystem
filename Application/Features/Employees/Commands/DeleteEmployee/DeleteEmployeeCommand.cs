using MediatR;

namespace Application.Features.Employees.Commands.DeleteEmployee;

public record DeleteEmployeeCommand(int EmployeeId) : IRequest, IRequest<bool>;
