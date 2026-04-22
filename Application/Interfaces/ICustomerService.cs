using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces;

public interface ICustomerService
{
    Task<PagedResponse<CustomerResponse>> GetAllAsync(int pageNumber, int pageSize);
    Task<CustomerResponse?> GetByIdAsync(int id);
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);
    Task<CustomerResponse?> UpdateAsync(int id, UpdateCustomerRequest request);
    Task<bool?> DeleteAsync(int id);
}