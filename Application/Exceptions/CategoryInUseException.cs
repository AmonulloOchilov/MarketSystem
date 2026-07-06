using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class CategoryInUseException : BaseException
{
    public CategoryInUseException(int id) : base($"Category {id} is in use",StatusCodes.Status400BadRequest)
    {
    }
}