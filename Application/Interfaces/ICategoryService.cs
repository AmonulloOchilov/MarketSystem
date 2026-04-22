using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Domain.Entities;

namespace Application.Interfaces;

public interface ICategoryService
{
    Task<PagedResponse<CategoryResponse>> GetAllAsync(int pageNumber, int pageSize);
    Task<CategoryResponse?> GetByIdAsync(int id);
    Task<CategoryResponse> CreateAsync(CreateCategoryRequest request);
    Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest request);
    Task<bool?> DeleteAsync(int id);
}