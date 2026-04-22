using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class CategoryNotFoundException : BaseException
{
    public CategoryNotFoundException(int id) 
        : base($"Category with ID {id} not found", StatusCodes.Status404NotFound)
    {
    }
}