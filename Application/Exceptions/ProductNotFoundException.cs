using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class ProductNotFoundException : BaseException
{
    public ProductNotFoundException(int id) 
        : base($"Product with ID {id} not found", StatusCodes.Status404NotFound)
    {
    }
}