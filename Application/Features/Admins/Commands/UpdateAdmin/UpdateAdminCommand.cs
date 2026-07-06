using Application.DTOs.Request;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Admins.Commands.UpdateAdmin;

public record UpdateAdminCommand(int AdminId, UpdateAdminRequest Request) : IRequest<AdminResponse>;