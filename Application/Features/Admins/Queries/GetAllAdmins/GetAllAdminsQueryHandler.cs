using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admins.Queries.GetAllAdmins;

public class GetAllAdminsQueryHandler : IRequestHandler<GetAllAdminsQuery, PagedResponse<AdminResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetAllAdminsQueryHandler> _logger;

    public GetAllAdminsQueryHandler(IAppDbContext dbContext, ILogger<GetAllAdminsQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<PagedResponse<AdminResponse>> Handle(GetAllAdminsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching admins. Page: {PageNumber}, Size: {PageSize}", request.PageNumber, request.PageSize);
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);
            return new PagedResponse<AdminResponse>();
        }

        var query = _dbContext.Admins.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);
        
        var admins = await query
            .OrderBy(a=>a.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a=> new AdminResponse()
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Username = a.Username,
                Role = a.Role,
                Email = a.Email
        }).ToListAsync(cancellationToken);
        
        _logger.LogInformation("Returned {Count} admins out of {Total}", admins.Count, totalCount);
        
        return new PagedResponse<AdminResponse>
        {
            Items = admins,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}