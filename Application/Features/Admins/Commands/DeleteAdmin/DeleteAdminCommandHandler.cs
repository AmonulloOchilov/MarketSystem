using Application.Exceptions;
using Application.Interfaces.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admins.Commands.DeleteAdmin;

public class DeleteAdminCommandHandler : IRequestHandler<DeleteAdminCommand, bool>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<DeleteAdminCommandHandler> _logger;

    public DeleteAdminCommandHandler(IAppDbContext dbContext, ILogger<DeleteAdminCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<bool> Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting admin with ID: {AdminId}", request.AdminId);

        var admin = await _dbContext.Admins.FirstOrDefaultAsync(a => a.Id == request.AdminId, cancellationToken);
        if (admin == null)
        {
            _logger.LogWarning("Admin with ID {AdminId} not found", request.AdminId);
            throw new AdminNotFoundException(request.AdminId);
        }

        _dbContext.Admins.Remove(admin);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Admin deleted successfully. ID: {AdminId}, Name: {AdminName}, Surname: {AdminSurname}",
            request.AdminId,
            admin.FirstName, admin.LastName);
        
        return true;
    }
}