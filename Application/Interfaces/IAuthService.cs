using Application.DTOs.Request;
using Application.DTOs.Response;

namespace Application.Interfaces;

public interface IAuthService
{
    Task<TokenResponse> LoginAsync(LoginRequest request);
    Task RegisterAsync(RegisterRequest request);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
}