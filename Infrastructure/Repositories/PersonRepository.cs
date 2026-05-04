using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly MarketDbContext _db;

    public PersonRepository(MarketDbContext db)
    {
        _db = db;
    }
    public async Task<Person?> GetByUsernameAsync(string username)
    {
        return await _db.Persons.FirstOrDefaultAsync(p => p.Username == username);
    }

    public async Task<Person?> AddAsync(Person person)
    {
        await _db.Persons.AddAsync(person);
        await _db.SaveChangesAsync();
        return person;
    }
}