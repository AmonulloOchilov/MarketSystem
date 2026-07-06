using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Data;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Application.Features.Admins.Commands.CreateAdmin;

public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, AdminResponse>
{
    private readonly IAppDbContext _dbContext;
    private readonly ILogger<CreateAdminCommandHandler> _logger;

    public CreateAdminCommandHandler(IAppDbContext dbContext, ILogger<CreateAdminCommandHandler> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task<AdminResponse> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating admin: {AdminName}", request.Request.FirstName);
        
        var username = request.Request.Username.Trim().ToLower();
        var email = request.Request.Email.Trim().ToLower();
        var phoneNumber = request.Request.PhoneNumber.Trim().ToLower();

        var exists =
            await _dbContext.Admins.AnyAsync(e => e.Username == username || e.Email == email, cancellationToken);
        
        if (exists)
        {
            _logger.LogWarning(
                "Admin already exists with username or email or phone number: {Username}, {Email}, {PhoneNumber}",
                username, email, phoneNumber);
            throw new AdminAlreadyExistsException();
        }
        var admin = new Admin
        {
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            Username = request.Request.Username,
            PhoneNumber = request.Request.PhoneNumber,
            Role = request.Request.Role,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password),
            Email = request.Request.Email
        };

         await _dbContext.Admins.AddAsync(admin, cancellationToken);
         await _dbContext.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Admin created successfully with ID: {AdminId}", admin.Id);

        return new AdminResponse
        {
            Id = admin.Id,
            FirstName = admin.FirstName,
            LastName = admin.LastName,
            Username = admin.Username,
            Role = admin.Role,
            Email = admin.Email,
            PhoneNumber = admin.PhoneNumber
        };
    }
}