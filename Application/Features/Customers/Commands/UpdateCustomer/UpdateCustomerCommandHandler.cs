using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, CustomerResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<UpdateCustomerCommandHandler> _logger;

    public UpdateCustomerCommandHandler(IAppDbContext dbContext, ILogger<UpdateCustomerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<CustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating customer with ID: {CustomerId}", request.CustomerId);

        var username = request.Request.Username.Trim().ToLower();
        var email = request.Request.Email.Trim().ToLower();

        var exists = await _dbContext.Customers
            .AnyAsync(c =>
                c.Id != request.CustomerId &&
                (c.Username == username || c.Email == email), cancellationToken);

        if (exists)
        {
            _logger.LogWarning("Customer already exists with username or email: {Username}, {Email}", username, email);
            throw new CustomerAlreadyExistsException();
        }
        
        var customer =
            await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", request.CustomerId);
            throw new CustomerNotFoundException(request.CustomerId);
        }

        customer.FirstName = request.Request.FirstName;
        customer.LastName = request.Request.LastName;
        customer.Username = username;
        customer.Role = request.Request.Role;
        customer.Email = email;
        customer.PhoneNumber = request.Request.PhoneNumber;

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Customer {CustomerId} updated successfully. Name: {CustomerName}, Surname: {CustomerSurname}", request.CustomerId,
            customer.FirstName, customer.LastName);

        return new CustomerResponse
        {
            Id = request.CustomerId,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Username = customer.Username,
            Role = customer.Role,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber
        };
    }
}