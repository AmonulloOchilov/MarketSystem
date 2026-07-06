using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class CategoryAlreadyExistsException : BaseException
{
    public CategoryAlreadyExistsException() : base("Category already exists", StatusCodes.Status400BadRequest)
    {
    }
}