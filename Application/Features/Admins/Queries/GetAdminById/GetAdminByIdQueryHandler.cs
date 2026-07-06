using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admins.Queries.GetAdminById;

public class GetAdminByIdQueryHandler : IRequestHandler<GetAdminByIdQuery, AdminResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<GetAdminByIdQueryHandler> _logger;

    public GetAdminByIdQueryHandler(IAppDbContext dbContext, ILogger<GetAdminByIdQueryHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<AdminResponse> Handle(GetAdminByIdQuery request, CancellationToken cancellationToken)
    {
        var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == request.AdminId, cancellationToken);
        if (admin == null)
        {
            _logger.LogWarning("Admin with ID {AdminId} not found", request.AdminId);
            throw new AdminNotFoundException(request.AdminId);
        }

        return new AdminResponse
        {
            Id = admin.Id,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            Username = admin.Username,
            Role = admin.Role,
            Email = admin.Email
        };
    }
}