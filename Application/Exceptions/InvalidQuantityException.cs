using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class InvalidQuantityException : BaseException
{
    public InvalidQuantityException(int productId) : base($"Invalid quantity for product: {productId}",
        StatusCodes.Status400BadRequest)

    {
        
    }
}