using Application.DTOs.Request;
using Application.DTOs.Response;
using Domain.Entities;

namespace Application.Interfaces;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync(int pageNumber, int pageSize);
    Task<ProductResponse?> GetByIdAsync(int id);
    Task<ProductResponse> CreateAsync(CreateProductRequest request);
    Task<ProductResponse?> UpdateAsync(int id, UpdateProductRequest request);
    Task<ProductResponse?> DeleteAsync(int id);
}