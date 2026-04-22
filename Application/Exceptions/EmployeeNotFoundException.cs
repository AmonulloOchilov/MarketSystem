using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class EmployeeNotFoundException : BaseException
{
    public EmployeeNotFoundException(int id) 
        : base($"Employee with ID {id} not found", StatusCodes.Status404NotFound)
    {
    }
}