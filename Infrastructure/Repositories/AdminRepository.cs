using Application.Common;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly MarketDbContext _db;
    private readonly ILogger<AdminRepository> _logger;

    public AdminRepository(MarketDbContext db, ILogger<AdminRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<PagedResult<Admin>> GetAllAsync(int pageNumber, int pageSize)
    {
        var totalCount = await _db.Admins.CountAsync();
        var items = await _db.Admins
            .OrderBy(c => c.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return new PagedResult<Admin>()
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task<Admin?> GetByIdAsync(int id)
    {
        var result = await _db.Admins.FindAsync(id);
        if (result == null)
        {
            _logger.LogWarning("Admin not found in database. Admin ID: {AdminId}", id);
            throw new AdminNotFoundException(id);
        }
        return result;
    }

    public async Task<Admin> AddAsync(Admin admin)
    {
        _logger.LogInformation("Adding admin to Database. Admin ID: {AdminId}", admin.Id);
        
        await _db.Admins.AddAsync(admin);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Admin successfully created with ID: {AdminId}", admin.Id);
        return admin;
    }

    public async Task<Admin?> UpdateAsync(Admin admin)
    {
        _logger.LogInformation("Updating admin. Admin ID: {AdminId}", admin.Id);
        
        _db.Admins.Update(admin);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Admin updated successfully. Admin ID: {AdminId}", admin.Id);
        return admin;
    }

    public async Task DeleteAsync(int id)
    {
        var admin = await _db.Admins.FindAsync(id);
        if (admin == null)
        {
            _logger.LogWarning("Cannot delete. Admin not found. Admin ID: {AdminId}", id);
            throw new AdminNotFoundException(id);
        }
        _logger.LogInformation("Deleting admin from database. Admin ID: {AdminId}", id);
        
        _db.Admins.Remove(admin);
        await _db.SaveChangesAsync();
        
        _logger.LogInformation("Admin deleted from database. Admin ID: {AdminId}", id);
    }
}