using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Orders.Queries.GetAllOrders;

public class GetAllOrdersQuery : IRequest<PagedResponse<OrderResponse>>
{
    public int PageNumber { get; }
    public int PageSize { get; }

    public GetAllOrdersQuery(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}