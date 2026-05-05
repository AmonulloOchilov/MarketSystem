using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class AdminNotFoundException : BaseException
{
    public AdminNotFoundException(int id) 
        : base($"Admin with ID {id} not found", StatusCodes.Status404NotFound)
    {
    }
}