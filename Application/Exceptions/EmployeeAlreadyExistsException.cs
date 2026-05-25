using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class EmployeeAlreadyExistsException : BaseException
{
    public EmployeeAlreadyExistsException() : base("Employee already exists", StatusCodes.Status400BadRequest)
    {
    }
}