using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Authentications.Commands.Refresh;

public record RefreshTokenCommand(RefreshTokenRequest Request) : IRequest<TokenResponse>;