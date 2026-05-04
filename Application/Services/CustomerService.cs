using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(ICustomerRepository repository, ILogger<CustomerService> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    public async Task<PagedResponse<CustomerResponse>> GetAllAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching employees. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            return new PagedResponse<CustomerResponse>();
        }
        
        var customers = await _repository.GetAllAsync(pageNumber, pageSize);

        var items = customers.Items.Select(c => new CustomerResponse
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            Username = c.Username,
            Role = c.Role,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber
        }).ToList();
        
        _logger.LogInformation("Returned {Count} customers out of {Total}", items.Count, customers.TotalCount);
        
        return new PagedResponse<CustomerResponse>()
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = customers.TotalCount
        };
    }

    public async Task<CustomerResponse?> GetByIdAsync(int id)
    {
        var customer = await _repository.GetByIdAsync(id);

        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", id);
            throw new CustomerNotFoundException(id);
        }

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

    public async Task<CustomerResponse> CreateAsync(CreateCustomerRequest request)
    {
        _logger.LogInformation("Creating customer. Name: {CustomerName}, Surname: {CustomerSurname}", request.FirstName,
            request.LastName);
        
        var customer = new Customer
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Role = request.Role,
            PasswordHash = request.Password,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber
        };

        var created = await _repository.AddAsync(customer);
        
        _logger.LogInformation("Customer created successfully with ID: {CustomerId}", created.Id);

        return new CustomerResponse
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Username = created.Username,
            Role = created.Role,
            Email = created.Email,
            PhoneNumber = created.PhoneNumber
        };
    }

    public async Task<CustomerResponse?> UpdateAsync(int id, UpdateCustomerRequest request)
    {
        _logger.LogInformation("Updating customer with ID: {CustomerId}", id);
        
        var customer = await _repository.GetByIdAsync(id);

        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", id);
            throw new CustomerNotFoundException(id);
        }

        customer.FirstName = request.FirstName;
        customer.LastName = request.LastName;
        customer.Username = request.Username;
        customer.Role = request.Role;
        customer.Email = request.Email;
        customer.PhoneNumber = request.PhoneNumber;

        var updated = await _repository.UpdateAsync(customer);

        _logger.LogInformation(
            "Customer {CustomerId} updated successfully. Name: {CustomerName}, Surname: {CustomerSurname}", id,
            updated.FirstName, updated.LastName);

        return new CustomerResponse
        {
            Id = updated.Id,
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            Username = updated.Username,
            Role = updated.Role,
            Email = updated.Email,
            PhoneNumber = updated.PhoneNumber
        };
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting customer with ID: {CustomerId}", id);
        
        var customer = await _repository.GetByIdAsync(id);

        if (customer == null)
        {
            _logger.LogWarning("Customer with ID {CustomerId} not found", id);
            throw new CustomerNotFoundException(id);
        }

        await _repository.DeleteAsync(id);

        _logger.LogInformation(
            "Customer deleted successfully. ID: {CustomerId}, Name: {CustomerName}, Surname: {CustomerSurname}", id,
            customer.FirstName, customer.LastName);
        
        return true;
    }
}