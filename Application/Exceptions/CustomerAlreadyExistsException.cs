using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class CustomerAlreadyExistsException : BaseException
{
    public CustomerAlreadyExistsException() : base("Customer already exists", StatusCodes.Status400BadRequest)
    {
    }
}