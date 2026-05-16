using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public class OrderAlreadyPaidException : BaseException
{
    public OrderAlreadyPaidException(int id) : base($"Order with ID {id} is already paid",
        StatusCodes.Status406NotAcceptable)

    {
    }
}