using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Reports.Queries.GetLowSellingProducts;

public record GetLowSellingProductsQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<LowSellingProductsResponse>>;