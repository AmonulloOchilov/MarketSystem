using Application.Interfaces.Repositories;
using Domain.Entities;
using Elasticsearch.Net;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly MarketDbContext _db;

    public RefreshTokenRepository(MarketDbContext db)
    {
        _db = db;
    }
    public async Task AddAsync(RefreshToken token)
    {
        await _db.RefreshTokens.AddAsync(token);
        await _db.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _db.RefreshTokens
            .Include(p => p.Person)
            .FirstOrDefaultAsync(x => x.Token == token);
    }
}