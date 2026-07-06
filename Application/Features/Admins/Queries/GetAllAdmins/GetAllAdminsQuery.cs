using Application.Common;
using Application.DTOs.Response;
using MediatR;

namespace Application.Features.Admins.Queries.GetAllAdmins;

public record GetAllAdminsQuery(int PageNumber, int PageSize) : IRequest<PagedResponse<AdminResponse>>;