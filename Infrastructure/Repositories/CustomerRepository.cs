using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly MarketDbContext _db;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(MarketDbContext db, ILogger<CustomerRepository> logger)
    {
        _db = db;
        _logger = logger;
    }
    public async Task<List<Customer>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _db.Customers.OrderBy(c => c.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize).ToListAsync();
    }

    public async Task<Customer> GetByIdAsync(int id)
    {
        var result = await _db.Customers.FindAsync(id);
        if (result == null)
        {
            _logger.LogWarning("Customer not found in database. Employee ID: {EmployeeId}", id);
            return null;
        }
        return result;
    }

    public async Task<Customer> AddAsync(Customer customer)
    {
        _logger.LogInformation("Adding customer to Database. Customer ID: {CustomerId}, Name: {CustomerName}, Surname: {CustomerSurname}",
            customer.Id, customer.FirstName, customer.LastName);
        
        await _db.Customers.AddAsync(customer);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Customer successfully created with ID: {CustomerId}", customer.Id);
        return customer;
    }

    public async Task<Customer?> UpdateAsync(Customer customer)
    {
        _logger.LogInformation("Updating customer. Customer ID: {CustomerId}", customer.Id);
        
        _db.Customers.Update(customer);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Customer updated successfully. Customer ID: {CustomerId}", customer.Id);
        return customer;
    }

    public async Task DeleteAsync(int id)
    {
        var customer = await _db.Customers.FindAsync(id);
        if (customer == null)
        {
            _logger.LogWarning("Cannot delete. Customer not found. Customer ID: {CustomerId}", id);
            return;
        }
        _logger.LogInformation("Deleting customer from database. Customer ID: {CustomerId}", id);
        
        _db.Customers.Remove(customer);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Customer deleted from database. Customer ID: {CustomerId}", id);
    }
}