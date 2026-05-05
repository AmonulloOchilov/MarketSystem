using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class AuthService : IAuthService
{
    private readonly IPersonRepository _personRepo;
    private readonly IRefreshTokenRepository _tokenRepo;
    private readonly JwtService _jwtService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IPersonRepository personRepo, IRefreshTokenRepository tokenRepo,
        JwtService jwtService, ILogger<AuthService> logger)
    {
        _personRepo = personRepo;
        _tokenRepo = tokenRepo;
        _jwtService = jwtService;
        _logger = logger;
    }

    public async Task<TokenResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Logging to the system");
        var user = await _personRepo.GetByUsernameAsync(request.Username);
        if (user == null)
        {
            _logger.LogWarning("User not found");
            throw new UserNotFoundException();
        }

        var isValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!isValid)
        {
            throw new InvalidPasswordException();
        }

        var accessToken = _jwtService.GenerateToken(user);

        var refreshToken = new RefreshToken()
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

    public async Task RegisterAsync(RegisterRequest request)
    {
        var user = new Person()
        {
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = request.Role,
            FirstName = request.FirstName,
            LastName = request.LastName
        };
        await _personRepo.AddAsync(user);
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var token = await _tokenRepo.GetByTokenAsync(refreshToken);
        if (token == null || token.IsRevoked || token.ExpiresAt < DateTime.UtcNow)
        {
            throw new InvalidRefreshTokenException();
        }

        var user = token.Person;

        var newAccessToken = _jwtService.GenerateToken(user);

        return new TokenResponse()
        {
            AccessToken = newAccessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
    }
}