using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<EmployeeResponse>> GetAllAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching categories. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            return new List<EmployeeResponse>();
        }
        
        var employees = await _repository.GetAllAsync(pageNumber, pageSize);

        return employees.Select(e => new EmployeeResponse
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            Position = e.Position,
            Email = e.Email
        }).ToList();
    }

    public async Task<EmployeeResponse?> GetByIdAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", id);
            return null;
        }

        return new EmployeeResponse
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            Email = employee.Email
        };
    }

    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
    {
        _logger.LogInformation("Creating employee. Name: {EmployeeName}, Surname: {EmployeeSurname}", request.FirstName,
            request.LastName);
        
        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Position = request.Position,
            Email = request.Email
        };

        var created = await _repository.AddAsync(employee);
        
        _logger.LogInformation("Employee created successfully with ID: {EmployeeId}", created.Id);

        return new EmployeeResponse
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Position = created.Position,
            Email = created.Email
        };
    }

    public async Task<EmployeeResponse?> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        _logger.LogInformation("Updating employee with ID: {EmployeeId}", id);
        
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {CustomerId} not found", id);
            return null;
        }

        employee.Id = id;
        employee.FirstName = request.FirstName;
        employee.LastName = request.LastName;
        employee.Position = request.Position;
        
        var updated = await _repository.UpdateAsync(employee);

        if (updated == null)
        {
            return null;
        }
        
        _logger.LogInformation(
            "Employee {EmployeeId} updated successfully. Name: {EmployeeName}, Surname: {EmployeeSurname}", id,
            updated.FirstName, updated.LastName);

        return new EmployeeResponse
        {
            Id = updated.Id,
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            Position = updated.Position,
            Email = updated.Email
        };
    }

    public async Task<EmployeeResponse?> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting employee with ID: {EmployeeId}", id);
        
        var employee = await _repository.GetByIdAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {EmployeeId} not found", id);
            return null;
        }

        await _repository.DeleteAsync(id);
        
        _logger.LogInformation("Employee deleted successfully with ID: {EmployeeId}", id);
        
        return new EmployeeResponse
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Position = employee.Position,
            Email = employee.Email
        };
    }
}