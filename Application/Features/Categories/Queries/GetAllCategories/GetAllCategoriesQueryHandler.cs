using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Categories.Queries.GetAllCategories;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, PagedResponse<CategoryResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

    public GetAllCategoriesQueryHandler(IAppDbContext dbContext, ILogger<GetAllCategoriesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<PagedResponse<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching categories. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);
            return new PagedResponse<CategoryResponse>();
        }
        
        var query = _dbContext.Categories.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var categories = await query
            .OrderBy(c => c.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new CategoryResponse()
            {
                Id = c.Id,
                Name = c.Name
            }).ToListAsync(cancellationToken);
        
        _logger.LogInformation("Returned {Count} categories out of {Total}", categories.Count, totalCount);
        
        return new PagedResponse<CategoryResponse>()
        {
            Items = categories,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}