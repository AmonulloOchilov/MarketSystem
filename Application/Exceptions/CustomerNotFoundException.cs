using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class CustomerNotFoundException : BaseException
{
    public CustomerNotFoundException(int id) 
        : base($"Customer with ID {id} not found", StatusCodes.Status404NotFound)
    {
    }
}