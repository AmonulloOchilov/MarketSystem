using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Authentications.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, TokenResponse>
{
    private readonly IPersonRepository _personRepo;
    private readonly IRefreshTokenRepository _tokenRepo;
    private readonly JwtService _jwtService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IPersonRepository personRepo, IRefreshTokenRepository tokenRepo, JwtService jwtService,
        ILogger<LoginCommandHandler> logger)
    {
        _personRepo = personRepo;
        _tokenRepo = tokenRepo;
        _jwtService = jwtService;
        _logger = logger;
    }
    public async Task<TokenResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Logging to the system");
        var user = await _personRepo.GetByUsernameAsync(request.Request.Username);
        if (user == null)
        {
            _logger.LogWarning("User not found");
            throw new UserNotFoundException();
        }

        var isValid = BCrypt.Net.BCrypt.Verify(request.Request.Password, user.PasswordHash);
        if (!isValid)
        {
            throw new InvalidPasswordException();
        }

        var accessToken = _jwtService.GenerateToken(user);

        var refreshToken = new Domain.Entities.RefreshToken()
        {
            Token = Guid.NewGuid().ToString(),
            PersonId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        await _tokenRepo.AddAsync(refreshToken);

        return new TokenResponse()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }
}