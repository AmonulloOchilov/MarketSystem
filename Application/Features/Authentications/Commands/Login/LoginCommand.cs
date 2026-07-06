using Application.DTOs.Response;
using MediatR;
using Application.DTOs.Request;

namespace Application.Features.Authentications.Commands.Login;

public record LoginCommand(LoginRequest Request) : IRequest<TokenResponse>;