using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, CustomerResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<CreateCustomerCommandHandler> _logger;

    public CreateCustomerCommandHandler(IAppDbContext dbContext, ILogger<CreateCustomerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<CustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating customer. Name: {CustomerName}, Surname: {CustomerSurname}", request.Request.FirstName,
            request.Request.LastName);
        
        var username = request.Request.Username.Trim().ToLower();
        var email = request.Request.Email.Trim().ToLower();

        var exists =
            await _dbContext.Customers.AnyAsync(e => e.Username == username || e.Email == email, cancellationToken);
        
        if (exists)
        {
            _logger.LogWarning("Customer already exists with username or email: {Username}, {Email}", username, email);
            throw new CustomerAlreadyExistsException();
        }
        
        var customer = new Customer
        {
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            Username = username,
            Role = request.Request.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password),
            Email = email,
            PhoneNumber = request.Request.PhoneNumber
        };

        await _dbContext.Customers.AddAsync(customer, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Customer created successfully with ID: {CustomerId}", customer.Id);

        return new CustomerResponse
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Username = customer.Username,
            Role = customer.Role,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber
        };
    }
}