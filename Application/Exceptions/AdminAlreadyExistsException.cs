using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class AdminAlreadyExistsException : BaseException
{
    public AdminAlreadyExistsException() : base("Category already exists", StatusCodes.Status400BadRequest)
    {
    }
}