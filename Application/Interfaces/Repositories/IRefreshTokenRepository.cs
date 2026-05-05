using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken> GetByTokenAsync(string token);
}