using Application.Interfaces.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly MarketDbContext _dbContext;
    private IDbContextTransaction _transaction;

    public UnitOfWork(MarketDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task BeginTransactionAsync()
    {
        _transaction = await _dbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync();
        await _transaction.CommitAsync();
    }

    public async Task RollbackAsync()
    {
        await _transaction.RollbackAsync();
    }
}