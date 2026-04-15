using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<List<Employee>> GetAllAsync(int pageNumber, int pageSize);

    Task<Employee?> GetByIdAsync(int id);

    Task<Employee> AddAsync(Employee employee);

    Task<Employee?> UpdateAsync(Employee employee);

    Task DeleteAsync(int id);
}