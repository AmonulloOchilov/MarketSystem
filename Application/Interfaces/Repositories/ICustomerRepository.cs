using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICustomerRepository
{
    Task<List<Customer>> GetAllAsync(int pageNumber, int pageSize);
    Task<Customer> GetByIdAsync(int id);
    Task<Customer> AddAsync(Customer customer);
    Task<Customer?> UpdateAsync(Customer customer);
    Task DeleteAsync(int id);
}