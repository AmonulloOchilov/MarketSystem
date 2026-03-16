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
            Name = a.FirstName,
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
            Name = admin.FirstName,
            Email = admin.Email
        };
    }

    public async Task<AdminResponse> CreateAsync(CreateAdminRequest request)
    {
        var admin = new Admin
        {
            FirstName = request.Name,
            Email = request.Email
        };

        var created = await _repository.AddAsync(admin);

        return new AdminResponse
        {
            Id = created.Id,
            Name = created.FirstName,
            Email = created.Email
        };
    }

    public async Task<AdminResponse?> UpdateAsync(int id, UpdateAdminRequest request)
    {
        var admin = new Admin
        {
            Id = id,
            FirstName = request.Name,
            Email = request.Email
        };

        var updated = await _repository.UpdateAsync(admin);

        if (updated == null)
            return null;

        return new AdminResponse
        {
            Id = updated.Id,
            Name = updated.FirstName,
            Email = updated.Email
        };
    }

    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}