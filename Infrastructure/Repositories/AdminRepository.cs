using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AdminRepository : IAdminRepository
{
    private readonly MarketDbContext _db;

    public AdminRepository(MarketDbContext db)
    {
        _db = db;
    }

    public async Task<List<Admin>> GetAllAsync()
    {
        return await _db.Admins.ToListAsync();
    }

    public async Task<Admin?> GetByIdAsync(int id)
    {
        return await _db.Admins.FindAsync(id);
    }

    public async Task<Admin> AddAsync(Admin admin)
    {
        await _db.Admins.AddAsync(admin);
        await _db.SaveChangesAsync();
        return admin;
    }

    public async Task<Admin?> UpdateAsync(Admin admin)
    {
        var existing = await _db.Admins.FindAsync(admin.Id);

        if (existing == null)
            return null;

        existing.FirstName = admin.FirstName;
        existing.Email = admin.Email;

        await _db.SaveChangesAsync();

        return existing;
    }

    public async Task DeleteAsync(int id)
    {
        var admin = await _db.Admins.FindAsync(id);

        if (admin != null)
        {
            _db.Admins.Remove(admin);
            await _db.SaveChangesAsync();
        }
    }
}