using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Products.Queries.GetAllProducts;

public record GetAllProductsQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<ProductResponse>>;