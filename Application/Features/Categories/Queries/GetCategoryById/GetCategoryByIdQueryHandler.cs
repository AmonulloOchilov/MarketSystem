using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(IAppDbContext dbContext, ILogger<GetCategoryByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<CategoryResponse> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _dbContext.Categories.FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken);

        if (category == null)
        {
            _logger.LogWarning("Category with ID {CustomerId} not found", request.CategoryId);
            throw new CategoryNotFoundException(request.CategoryId);
        }

        return new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name
        };
    }
}