using MediatR;

namespace Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(int CategoryId) : IRequest, IRequest<bool>;