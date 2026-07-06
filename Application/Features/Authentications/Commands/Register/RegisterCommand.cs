using Application.DTOs.Request;
using MediatR;

namespace Application.Features.Authentications.Commands.Register;

public record RegisterCommand(RegisterRequest Request) : IRequest<bool>
{
    
}