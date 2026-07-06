using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Customers.Commands.DeleteCustomer;

public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<DeleteCustomerCommandHandler> _logger;

    public DeleteCustomerCommandHandler(IAppDbContext dbContext, ILogger<DeleteCustomerCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting customer with ID: {CustomerId}", request.CustomerId);

        var customer =
            await _dbContext.Customers.FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", request.CustomerId);
            throw new CustomerNotFoundException(request.CustomerId);
        }

        _dbContext.Customers.Remove(customer);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Customer deleted successfully. ID: {CustomerId}, Name: {CustomerName}, Surname: {CustomerSurname}",
            request.CustomerId,
            customer.FirstName, customer.LastName);
        
        return true;
    }
}