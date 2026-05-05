using Application.Common;
using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _repository;
    private readonly ILogger<AdminService> _logger;

    public AdminService(IAdminRepository repository, ILogger<AdminService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<PagedResponse<AdminResponse>> GetAllAsync(int pageNumber, int pageSize)
    {
        _logger.LogInformation("Fetching admins. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
        if (pageNumber <= 0 || pageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}", pageNumber, pageSize);
            return new PagedResponse<AdminResponse>();
        }
        
        var admins = await _repository.GetAllAsync(pageNumber, pageSize);
        
        var items = admins.Items.Select(a => new AdminResponse()
        {
            Id = a.Id,
            FirstName = a.FirstName,
            LastName = a.LastName,
            Username = a.Username,
            Role = a.Role,
            Email = a.Email
        }).ToList();
        
        _logger.LogInformation("Returned {Count} admins out of {Total}", items.Count, admins.TotalCount);
        
        return new PagedResponse<AdminResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = admins.TotalCount
        };
    }

    public async Task<AdminResponse?> GetByIdAsync(int id)
    {
        var admin = await _repository.GetByIdAsync(id);

        if (admin == null)
        {
            _logger.LogWarning("Admin with ID {AdminId} not found", id);
            throw new AdminNotFoundException(id);
        }

        return new AdminResponse
        {
            Id = admin.Id,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            Username = admin.Username,
            Role = admin.Role,
            Email = admin.Email
        };
    }

    public async Task<AdminResponse> CreateAsync(CreateAdminRequest request)
    {
        _logger.LogInformation("Creating admin: {AdminName}", request.FirstName);
        
        var admin = new Admin
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Role = request.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Email = request.Email
        };

        var created = await _repository.AddAsync(admin);
        
        _logger.LogInformation("Admin created successfully with ID: {AdminId}", created.Id);

        return new AdminResponse
        {
            Id = created.Id,
            FirstName = created.FirstName,
            LastName = created.LastName,
            Username = created.Username,
            Role = created.Role,
            Email = created.Email
        };
    }

    public async Task<AdminResponse?> UpdateAsync(int id, UpdateAdminRequest request)
    {
        _logger.LogInformation("Updating admin with ID: {AdminId}", id);
        
        var admin = new Admin
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Username = request.Username,
            Email = request.Email
        };

        var updated = await _repository.UpdateAsync(admin);
        if (updated == null)
        {
            _logger.LogWarning("Admin with {AdminId} not found", id);
            throw new AdminNotFoundException(id);
        }

        _logger.LogInformation("Admin {AdminId} updated successfully. Name: {AdminName}, Surname: {AdminSurname}", id,
            updated.FirstName, updated.LastName);
        
        return new AdminResponse
        {
            Id = updated.Id,
            FirstName = updated.FirstName,
            LastName = updated.LastName,
            Username = updated.Username,
            Email = updated.Email
        };
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting admin with ID: {AdminId}", id);
        
        var admin = await _repository.GetByIdAsync(id);
        if (admin == null)
        {
            _logger.LogWarning("Admin with ID {AdminId} not found", id);
            throw new AdminNotFoundException(id);
        }

        await _repository.DeleteAsync(id);
        
        _logger.LogInformation("Admin deleted successfully with ID: {AdminId}", id);
        
        return true;
    }
}