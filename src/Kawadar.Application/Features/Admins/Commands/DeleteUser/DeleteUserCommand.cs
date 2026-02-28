using Kawadar.Domain.Common.Results;
using MediatR;

namespace Kawadar.Application.Features.Admins.Commands.DeleteUser
{
    public record DeleteUserCommand(string userName) : IRequest<Result<Deleted>>;
}
