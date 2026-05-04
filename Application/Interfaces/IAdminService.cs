using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces;

public interface IAdminService
{
    Task<PagedResponse<AdminResponse>> GetAllAsync(int pageNumber, int pageSize);

    Task<AdminResponse?> GetByIdAsync(int id);

    Task<AdminResponse> CreateAsync(CreateAdminRequest request);

    Task<AdminResponse?> UpdateAsync(int id, UpdateAdminRequest request);
    Task<bool?> DeleteAsync(int id);
}