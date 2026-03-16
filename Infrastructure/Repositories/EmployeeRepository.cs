using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly MarketDbContext _db;

    public EmployeeRepository(MarketDbContext db)
    {
        _db = db;
    }

    public async Task<List<Employee>> GetAllAsync()
    {
        return await _db.Employees.ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _db.Employees.FindAsync(id);
    }

    public async Task<Employee> AddAsync(Employee employee)
    {
        await _db.Employees.AddAsync(employee);
        await _db.SaveChangesAsync();
        return employee;
    }

    public async Task<Employee?> UpdateAsync(Employee employee)
    {
        var existing = await _db.Employees.FindAsync(employee.Id);

        if (existing == null)
            return null;

        existing.FirstName = employee.FirstName;
        existing.Position = employee.Position;

        await _db.SaveChangesAsync();

        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var employee = await _db.Employees.FindAsync(id);

        if (employee != null)
        {
            _db.Employees.Remove(employee);
            await _db.SaveChangesAsync();
        }
    }
}