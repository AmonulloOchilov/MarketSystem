using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class OrderNotFoundException : BaseException
{
    public OrderNotFoundException(int id) 
        : base($"Order with ID {id} not found", StatusCodes.Status404NotFound)
    {
    }
}