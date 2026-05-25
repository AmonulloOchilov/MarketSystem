using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Employees.Commands.UpdateEmployee;

public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, EmployeeResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<UpdateEmployeeCommandHandler> _logger;

    public UpdateEmployeeCommandHandler(IAppDbContext dbContext, ILogger<UpdateEmployeeCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<EmployeeResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating employee with ID: {EmployeeId}", request.EmployeeId);

        var username = request.Request.Username.Trim().ToLower();
        var email = request.Request.Email.Trim().ToLower();

        var exists = await _dbContext.Employees
            .AnyAsync(e =>
                    e.Id != request.EmployeeId &&
                    (e.Username == username || e.Email == email), cancellationToken);

        if (exists)
        {
            throw new EmployeeAlreadyExistsException();
        }
        
        var employee =
            await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", request.EmployeeId);
            throw new EmployeeNotFoundException(request.EmployeeId);
        }

        employee.FirstName = request.Request.FirstName;
        employee.LastName = request.Request.LastName;
        employee.Username = request.Request.Username.Trim().ToLower();
        employee.Email = request.Request.Email.Trim().ToLower(); 
        employee.Role = request.Request.Role;
        employee.Position = request.Request.Position;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Employee {EmployeeId} updated successfully. Name: {EmployeeName}, Surname: {EmployeeSurname}", request.EmployeeId,
            employee.FirstName, employee.LastName);

        return new EmployeeResponse
        {
            Id = request.EmployeeId,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Username = employee.Username,
            Role = employee.Role,
            Position = employee.Position,
            Email = employee.Email
        };
    }
}