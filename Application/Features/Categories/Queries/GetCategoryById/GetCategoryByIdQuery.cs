using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(int CategoryId) : IRequest<CategoryResponse>;