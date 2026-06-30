using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(IAppDbContext dbContext, ILogger<CreateCategoryCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating category: {CategoryName}", request.Request.Name);
        
        var name = request.Request.Name.Trim().ToLower();

        var exists =
            await _dbContext.Categories.AnyAsync(c => c.Name == name, cancellationToken);
        
        if (exists)
        {
            _logger.LogWarning("Category already exists with name: {Name}", name);
            throw new CategoryAlreadyExistsException();
        }
        
        var category = new Category()
        {
            Name = name
        };

        await _dbContext.Categories.AddAsync(category, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Category created successfully with ID: {CategoryId}", category.Id);

        return new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}