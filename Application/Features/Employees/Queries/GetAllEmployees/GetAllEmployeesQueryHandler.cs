using Application.Common;
using Application.DTOs.Response;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Employees.Queries.GetAllEmployees;

public class GetAllEmployeesQueryHandler : IRequestHandler<GetAllEmployeesQuery, PagedResponse<EmployeeResponse>>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetAllEmployeesQueryHandler> _logger;

    public GetAllEmployeesQueryHandler(IAppDbContext dbContext, ILogger<GetAllEmployeesQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    public async Task<PagedResponse<EmployeeResponse>> Handle(GetAllEmployeesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching employees. Page: {PageNumber}, Size: {PageSize}", request.PageNumber,
            request.PageSize);
        
        if (request.PageNumber <= 0 || request.PageSize <= 0)
        {
            _logger.LogWarning("Invalid pagination parameters. Page: {PageNumber}, Size: {PageSize}",
                request.PageNumber, request.PageSize);
            return new PagedResponse<EmployeeResponse>();
        }

        var query = _dbContext.Employees.AsQueryable();

        var totalCount = await query.CountAsync(cancellationToken);

        var employees = await query
            .OrderBy(e => e.Id)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(e => new EmployeeResponse()
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Username = e.Username,
                Role = e.Role,
                Position = e.Position,
                Email = e.Email,
                PhoneNumber = e.PhoneNumber
            }).ToListAsync(cancellationToken);
        
        _logger.LogInformation("Returned {Count} employees out of {Total}", employees.Count, totalCount);
        
        return new PagedResponse<EmployeeResponse>()
        {
            Items = employees,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount
        };
    }
}