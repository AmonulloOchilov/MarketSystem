using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces;

public interface IEmployeeService
{
    Task<PagedResponse<EmployeeResponse>> GetAllAsync(int pageNumber, int pageSize);

    Task<EmployeeResponse?> GetByIdAsync(int id);

    Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request);

    Task<EmployeeResponse?> UpdateAsync(int id, UpdateEmployeeRequest request);

    Task<bool?> DeleteAsync(int id);
}