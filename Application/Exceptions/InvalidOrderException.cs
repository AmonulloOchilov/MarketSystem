using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class InvalidOrderException : BaseException
{
    public InvalidOrderException(string message) : base(message, StatusCodes.Status400BadRequest)
    {
    }
}