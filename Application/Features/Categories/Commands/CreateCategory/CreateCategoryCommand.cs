using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryResponse>;