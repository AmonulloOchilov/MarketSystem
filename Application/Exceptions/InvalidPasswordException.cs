using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class InvalidPasswordException : BaseException
{
    public InvalidPasswordException() 
        : base("Invalid password", StatusCodes.Status400BadRequest)
    {
        
    }
}