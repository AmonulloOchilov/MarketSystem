using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Admins.Queries.GetAdminById;

public record GetAdminByIdQuery(int AdminId) : IRequest<AdminResponse>;