using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<PagedResult<Category>> GetAllAsync(int pageNumber, int pageSize);
    Task<Category?> GetByIdAsync(int id);
    Task<Category> AddAsync(Category category);
    Task<Category?> UpdateAsync(Category category);
    Task DeleteAsync(int id);
}