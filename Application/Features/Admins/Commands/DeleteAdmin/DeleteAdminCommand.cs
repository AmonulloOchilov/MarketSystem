using MediatR;

namespace Application.Features.Admins.Commands.DeleteAdmin;

public record DeleteAdminCommand(int AdminId) : IRequest,IRequest<bool>;