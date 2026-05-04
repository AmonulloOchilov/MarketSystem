using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class InvalidRefreshTokenException : BaseException
{
    public InvalidRefreshTokenException() 
        : base("Invalid refresh token", StatusCodes.Status400BadRequest)
    {
        
    }
}