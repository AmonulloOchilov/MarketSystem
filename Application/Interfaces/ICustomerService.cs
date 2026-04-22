using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces;

public interface ICustomerService
{
    Task<List<CustomerResponse>> GetAllAsync();
    Task<CustomerResponse?> GetByIdAsync(int id);
    Task<CustomerResponse> CreateAsync(CreateCustomerRequest request);
    Task<CustomerResponse?> UpdateAsync(int id, UpdateCustomerRequest request);
    Task<CustomerResponse?> DeleteAsync(int id);
}