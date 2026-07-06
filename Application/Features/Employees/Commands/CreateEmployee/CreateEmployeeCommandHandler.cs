using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, EmployeeResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<CreateEmployeeCommandHandler> _logger;

    public CreateEmployeeCommandHandler(IAppDbContext dbContext, ILogger<CreateEmployeeCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<EmployeeResponse> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating employee. Name: {EmployeeName}, Surname: {EmployeeSurname}", request.Request.FirstName,
            request.Request.LastName);

        var username = request.Request.Username.Trim().ToLower();
        var email = request.Request.Email.Trim().ToLower();
        var phoneNumber = request.Request.PhoneNumber.Trim().ToLower();

        var exists =
            await _dbContext.Employees.AnyAsync(
                e => e.Username == username || e.Email == email || e.PhoneNumber == phoneNumber, cancellationToken);
        
        if (exists)
        {
            _logger.LogWarning("Employee already exists with username or email or phone number: {Username}, {Email}, {PhoneNumber}", username, email, phoneNumber);
            throw new EmployeeAlreadyExistsException();
        }
        
        var employee = new Employee
        {
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            Username = username,
            Role = request.Request.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password),
            Position = request.Request.Position,
            Email = email,
            PhoneNumber = phoneNumber
        };

        await _dbContext.Employees.AddAsync(employee, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Employee created successfully with ID: {EmployeeId}", employee.Id);

        return new EmployeeResponse
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Username = employee.Username,
            Role = employee.Role,
            Position = employee.Position,
            Email = employee.Email,
            PhoneNumber = employee.PhoneNumber
        };
    }
}