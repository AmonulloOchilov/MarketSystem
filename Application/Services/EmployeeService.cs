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
            Name = e.FirstName,
            Position = e.Position
        }).ToList();
    }

    public async Task<EmployeeResponse?> GetByIdAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
            return null;

        return new EmployeeResponse
        {
            Id = employee.Id,
            Name = employee.FirstName,
            Position = employee.Position
        };
    }

    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
    {
        var employee = new Employee
        {
            FirstName = request.Name,
            Position = request.Position
        };

        var created = await _repository.AddAsync(employee);

        return new EmployeeResponse
        {
            Id = created.Id,
            Name = created.FirstName,
            Position = created.Position
        };
    }

    public async Task<EmployeeResponse?> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        var employee = new Employee
        {
            Id = id,
            FirstName = request.Name,
            Position = request.Position
        };

        var updated = await _repository.UpdateAsync(employee);

        if (updated == null)
            return null;

        return new EmployeeResponse
        {
            Id = updated.Id,
            Name = updated.FirstName,
            Position = updated.Position
        };
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}