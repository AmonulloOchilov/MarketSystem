using Application.Common;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IAdminRepository
{
    Task<PagedResult<Admin>> GetAllAsync(int pageNumber, int pageSize);
    Task<Admin?> GetByIdAsync(int id);
    Task<Admin> AddAsync(Admin admin);
    Task<Admin?> UpdateAsync(Admin admin);
    Task DeleteAsync(int id);
}