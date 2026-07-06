using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<CategoryResponse>>;