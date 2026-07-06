using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admins.Commands.UpdateAdmin;

public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand, AdminResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<UpdateAdminCommandHandler> _logger;

    public UpdateAdminCommandHandler(IAppDbContext dbContext, ILogger<UpdateAdminCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<AdminResponse> Handle(UpdateAdminCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating admin with ID: {AdminId}", request.AdminId);
        
        var username = request.Request.Username.Trim().ToLower();
        var email = request.Request.Email.Trim().ToLower();

        var exists = await _dbContext.Admins
            .AnyAsync(c =>
                c.Id != request.AdminId &&
                (c.Username == username || c.Email == email), cancellationToken);

        if (exists)
        {
            _logger.LogWarning("Admin already exists with username or email: {Username}, {Email}", username, email);
            throw new AdminAlreadyExistsException();
        }

        var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == request.AdminId, cancellationToken);
        
        if (admin == null)
        {
            _logger.LogWarning("Admin with ID {AdminId} not found", request.AdminId);
            throw new AdminNotFoundException(request.AdminId);
        }

        admin.FirstName = request.Request.FirstName;
        admin.LastName = request.Request.LastName;
        admin.Username = username;
        admin.Role = request.Request.Role;
        admin.Email = email;

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Admin {AdminId} updated successfully. Name: {AdminName}, Surname: {AdminSurname}", request.AdminId,
            admin.FirstName, admin.LastName);
        
        return new AdminResponse
        {
            Id = request.AdminId,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            Username = admin.Username,
            Role = admin.Role,
            Email = admin.Email
        };
    }
}