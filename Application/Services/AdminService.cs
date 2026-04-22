using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Application.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _repository;

    public AdminService(IAdminRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<AdminResponse>> GetAllAsync()
    {
        var admins = await _repository.GetAllAsync();

        return admins.Select(a => new AdminResponse
        {
            Id = a.Id,
            FirstName = a.FirstName,
            LastName = a.LastName,
            Role = a.Role,
            Email = a.Email
        }).ToList();
    }

    public async Task<AdminResponse?> GetByIdAsync(int id)
    {
        var admin = await _repository.GetByIdAsync(id);

        if (admin == null)
            return null;

        return new AdminResponse
        {
            Id = admin.Id,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            Role = admin.Role,
            Email = admin.Email
        };
    }

    public async Task<AdminResponse> CreateAsync(CreateAdminRequest request)
    {
        var admin = new Admin
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            Email = request.Email
        };

        var created = await _repository.AddAsync(admin);

        return new AdminResponse
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Role = created.Role,
            Email = created.Email
        };
    }

    public async Task<AdminResponse?> UpdateAsync(int id, UpdateAdminRequest request)
    {
        var admin = new Admin
        {
            Id = id,
            FirstName = request.FirstName,
            Email = request.Email
        };

        var updated = await _repository.UpdateAsync(admin);

        if (updated == null)
            return null;

        return new AdminResponse
        {
            Id = updated.Id,
            FirstName = updated.FirstName,
            Email = updated.Email
        };
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}