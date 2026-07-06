using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Orders.Queries.GetAllOrders;

public record GetAllOrdersQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<OrderResponse>>;