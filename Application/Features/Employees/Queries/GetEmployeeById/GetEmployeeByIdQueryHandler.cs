using Application.DTOs.Response;
using Application.Exceptions;
using Application.Features.Employees.Queries.GetAllEmployees;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetEmployeeByIdQueryHandler> _logger;

    public GetEmployeeByIdQueryHandler(IAppDbContext dbContext, ILogger<GetEmployeeByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<EmployeeResponse> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", request.EmployeeId);
            throw new EmployeeNotFoundException(request.EmployeeId);
        }

        return new EmployeeResponse
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Username = employee.Username,
            Role = employee.Role,
            Position = employee.Position,
            Email = employee.Email
        };
    }
}