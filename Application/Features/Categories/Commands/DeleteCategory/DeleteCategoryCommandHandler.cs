using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<DeleteCategoryCommandHandler> _logger;

    public DeleteCategoryCommandHandler(IAppDbContext dbContext, ILogger<DeleteCategoryCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting category with ID: {CategoryId}", request.CategoryId);

        var category =
            await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", request.CategoryId);
            throw new CategoryNotFoundException(request.CategoryId);
        }

        var isUsed = await _dbContext.Products
            .AnyAsync(p => p.CategoryId == request.CategoryId, cancellationToken);
        
        if (isUsed)
        {
            throw new CategoryInUseException(request.CategoryId);
        }
        
        _dbContext.Categories.Remove(category);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Category deleted successfully. ID: {CategoryId}, Name: {CategoryName}",
            request.CategoryId, category.Name);
        
        return true;
    }
}