using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IPersonRepository
{
    Task<Person?> GetByUsernameAsync(string username);
    Task<Person?> AddAsync(Person person);
}