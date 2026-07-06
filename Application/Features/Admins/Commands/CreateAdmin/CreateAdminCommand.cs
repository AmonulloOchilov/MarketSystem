using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Admins.Commands.CreateAdmin;

public record CreateAdminCommand(CreateAdminRequest Request) : IRequest<AdminResponse>;