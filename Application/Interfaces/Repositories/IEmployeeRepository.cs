using Application.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<PagedResult<Employee>> GetAllAsync(int pageNumber, int pageSize);

    Task<Employee?> GetByIdAsync(int id);

    Task<Employee> AddAsync(Employee employee);

    Task<Employee?> UpdateAsync(Employee employee);

    Task DeleteAsync(int id);
}