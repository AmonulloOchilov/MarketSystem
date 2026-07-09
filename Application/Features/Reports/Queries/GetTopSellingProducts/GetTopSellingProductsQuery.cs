using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Reports.Queries.GetTopSellingProducts;

public record GetTopSellingProductsQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<TopSellingProductsResponse>>;