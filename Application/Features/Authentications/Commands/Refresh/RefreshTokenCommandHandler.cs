using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Authentications.Commands.Refresh;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, TokenResponse>
{
    private readonly IRefreshTokenRepository _tokenRepo;
    private readonly JwtService _jwtService;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(IRefreshTokenRepository tokenRepo, JwtService jwtService,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _tokenRepo = tokenRepo;
        _jwtService = jwtService;
        _logger = logger;
    }
    public async Task<TokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Refreshing token");

        var token = await _tokenRepo.GetByTokenAsync(request.Request.RefreshToken);

        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
        {
            _logger.LogWarning("Invalid refresh token");
            throw new InvalidRefreshTokenException();
        }

        var user = token.Person;

        var newAccessToken = _jwtService.GenerateToken(user);

        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = request.Request.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }
}