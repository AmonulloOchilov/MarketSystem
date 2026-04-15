using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly MarketDbContext _db;
    private readonly ILogger<EmployeeRepository> _logger;

    public EmployeeRepository(MarketDbContext db, ILogger<EmployeeRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<Employee>> GetAllAsync(int pageNumber, int pageSize)
    {
        return await _db.Employees.OrderBy(e=>e.Id).Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        var result = await _db.Employees
            .FirstOrDefaultAsync(e => e.Id == id);
        
        if (result == null)
        {
            _logger.LogWarning("Employee not found in database. Employee ID: {EmployeeId}", id);
            return null;
        }
        return result;
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        _logger.LogInformation("Adding employee to Database. Employee ID: {EmployeeId}, Name: {EmployeeName}, Position: {Position}",
            employee.Id, employee.FirstName, employee.Position);
        
        await _db.Employees.AddAsync(employee);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Employee successfully created with ID: {EmployeeId}", employee.Id);
        return employee;
    }

    public async Task<Employee?> UpdateAsync(Employee employee)
    {
        var existing = await _db.Employees.FindAsync(employee.Id);

        if (existing == null)
        {
            _logger.LogWarning("Cannot update. Employee not found. EmployeeId: {EmployeeId}", employee.Id);
            return null;
        }
        
        _logger.LogInformation("Updating employee. Employee ID: {EmployeeId}", employee.Id);

        existing.FirstName = employee.FirstName;
        existing.Position = employee.Position;

        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Employee updated successfully. Employee ID: {EmployeeId}", existing.Id);
        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _db.Employees.FindAsync(id);
        if (employee == null)
        {
            _logger.LogWarning("Cannot delete. Employee not found. Employee ID: {EmployeeId}", id);
            return;
        }
        _logger.LogInformation("Deleting employee from database. Employee ID: {EmployeeId}", id);
        
        _db.Employees.Remove(employee);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Employee deleted from database. Employee ID: {EmployeeId}", id);
    }
}