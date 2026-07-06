using Application.Interfaces.Repositories;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Authentications.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, bool>
{
    private readonly IPersonRepository _personRepo;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(IPersonRepository personRepo, ILogger<RegisterCommandHandler> logger)
    {
        _personRepo = personRepo;
        _logger = logger;
    }
    
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Registering user: {Username}", request.Request.Username);

        var exists = await _personRepo.GetByUsernameAsync(request.Request.Username);

        if (exists != null)
        {
            throw new Exception("User already exists");
        }

        var user = new Person
        {
            Username = request.Request.Username.Trim(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Request.Password),
            FirstName = request.Request.FirstName,
            LastName = request.Request.LastName,
            Role = request.Request.Role
        };

        await _personRepo.AddAsync(user);

        _logger.LogInformation("User created successfully: {Username}", request.Request.Username);

        return true;
    }
}