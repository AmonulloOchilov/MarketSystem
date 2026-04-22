using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces;

public interface IAdminService
{
    Task<List<AdminResponse>> GetAllAsync();

    Task<AdminResponse?> GetByIdAsync(int id);

    Task<AdminResponse> CreateAsync(CreateAdminRequest request);

    Task<AdminResponse?> UpdateAsync(int id, UpdateAdminRequest request);

    Task DeleteAsync(int id);
}