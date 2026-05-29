using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Employees.Commands.DeleteEmployee;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, bool>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<DeleteEmployeeCommandHandler> _logger;

    public DeleteEmployeeCommandHandler(IAppDbContext dbContext, ILogger<DeleteEmployeeCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<bool> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting employee with ID: {EmployeeId}", request.EmployeeId);

        var employee = await _dbContext.Employees.FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);
        
        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", request.EmployeeId);
            throw new EmployeeNotFoundException(request.EmployeeId);
        }

        _dbContext.Employees.Remove(employee);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Employee deleted successfully with ID: {EmployeeId}", request.EmployeeId);

        return true;
    }
}