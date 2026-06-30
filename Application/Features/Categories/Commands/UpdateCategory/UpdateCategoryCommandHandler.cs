using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(IAppDbContext dbContext, ILogger<UpdateCategoryCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<CategoryResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating category with ID: {CategoryId}", request.CategoryId);

        var name = request.Request.Name.Trim().ToLower();

        var exists = await _dbContext.Categories
            .AnyAsync(c =>
                c.Id != request.CategoryId && c.Name == name, cancellationToken);

        if (exists)
        {
            _logger.LogWarning("Category already exists with name: {Name}", name);
            throw new CategoryAlreadyExistsException();
        }
        
        var category =
            await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);
        
        if (category == null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", request.CategoryId);
            throw new CategoryNotFoundException(request.CategoryId);
        }

        category.Name = name;
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Category {CategoryId} updated successfully. Name: {CategoryName}", request.CategoryId,
            category.Name);
        
        return new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}