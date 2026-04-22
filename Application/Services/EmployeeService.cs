using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;

    public EmployeeService(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<EmployeeResponse>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();

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
        var employee = new Employee
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Position = request.Position,
            Email = request.Email
        };

        var created = await _repository.AddAsync(employee);

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
        var employee = new Employee
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Position = request.Position
        };

        var updated = await _repository.UpdateAsync(employee);

        if (updated == null)
        {
            return null;
        }

        return new EmployeeResponse
        {
            Id = updated.Id,
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            Position = updated.Position,
            Email = updated.Email
        };
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}