using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(int CategoryId, UpdateCategoryRequest Request) : IRequest<CategoryResponse>;