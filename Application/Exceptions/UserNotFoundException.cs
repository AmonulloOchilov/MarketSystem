using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class UserNotFoundException : BaseException
{
    public UserNotFoundException() 
        : base("User not found", StatusCodes.Status404NotFound)
    {
    }
}