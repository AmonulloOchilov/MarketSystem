using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class ProductInUseException : BaseException
{
    public ProductInUseException(int id) : base($"Product {id} is in use", StatusCodes.Status400BadRequest)
    {
    }
}